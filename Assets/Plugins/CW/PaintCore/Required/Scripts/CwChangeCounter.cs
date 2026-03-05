using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using CW.Common;
#if !UNITY_WEBGL
using Unity.Jobs;
using Unity.Burst;
#endif

namespace PaintCore
{
    [ExecuteInEditMode]
    [HelpURL(CwCommon.HelpUrlPrefix + "CwChangeCounter")]
    [AddComponentMenu(CwCommon.ComponentMenuPrefix + "Change Counter")]
    public class CwChangeCounter : CwPaintableTextureMonitorMask
    {
        public static LinkedList<CwChangeCounter> Instances = new LinkedList<CwChangeCounter>(); private LinkedListNode<CwChangeCounter> instancesNode;

        public float Threshold { set { if (threshold != value) { threshold = value; MarkChangeReaderAsDirty(); } } get { return threshold; } }
        [Range(0.0f, 1.0f)][SerializeField] private float threshold = 0.1f;
        public Texture Texture { set { if (texture != value) { texture = value; MarkChangeReaderAsDirty(); } } get { return texture; } }
        [SerializeField] private Texture texture;
        public Color Color { set { if (color != value) { color = value; MarkChangeReaderAsDirty(); } } get { return color; } }
        [SerializeField] private Color color = Color.white;
        public int Count { get { return count; } }
        [SerializeField] private int count;
        public float Ratio { get { return total > 0 ? count / (float)total : 0.0f; } }

        [System.NonSerialized] private CwReader changeReader;
        [SerializeField] protected NativeArray<Color32> changePixels;

        // ── Job result ───────────────────────────────────────────────────────
#if !UNITY_WEBGL
        private NativeArray<int> _jobResults; // [0] = count, [1] = total
        private JobHandle _jobHandle;
        private bool _jobPending;
        private int _pendingBoost;
#endif

        public CwReader ChangeReader => changeReader;

        public bool HasRead => MaskReader != null && MaskReader.ReadCount > 0
                            && CurrentReader != null && CurrentReader.ReadCount > 0
                            && changeReader != null && changeReader.ReadCount > 0;

        // ── Burst Job ────────────────────────────────────────────────────────

#if !UNITY_WEBGL
        [BurstCompile]
        private struct CountChangedPixelsJob : IJob
        {
            [ReadOnly] public NativeArray<Color32> CurrentPixels;
            [ReadOnly] public NativeArray<byte> MaskPixels;
            [ReadOnly] public NativeArray<Color32> ChangePixels;
            public byte Threshold32;

            public NativeArray<int> Results; // [0] = count, [1] = total

            public void Execute()
            {
                int count = 0;
                int total = 0;

                for (int i = 0; i < CurrentPixels.Length; i++)
                {
                    if (MaskPixels[i] > 127)
                    {
                        total++;

                        Color32 cur = CurrentPixels[i];
                        Color32 chg = ChangePixels[i];

                        int distance = 0;
                        distance += cur.r > chg.r ? cur.r - chg.r : chg.r - cur.r;
                        distance += cur.g > chg.g ? cur.g - chg.g : chg.g - cur.g;
                        distance += cur.b > chg.b ? cur.b - chg.b : chg.b - cur.b;
                        distance += cur.a > chg.a ? cur.a - chg.a : chg.a - cur.a;

                        if (distance > Threshold32)
                            count++;
                    }
                }

                Results[0] = count;
                Results[1] = total;
            }
        }
#endif

        public void MarkChangeReaderAsDirty()
        {
            if (changeReader != null) changeReader.MarkAsDirty();
        }

        public static long GetTotal(ICollection<CwChangeCounter> counters = null)
        {
            var total = 0L; foreach (var counter in counters ?? Instances) { if (counter != null) total += counter.total; }
            return total;
        }

        public static long GetCount(ICollection<CwChangeCounter> counters = null)
        {
            var solid = 0L; foreach (var counter in counters ?? Instances) { if (counter != null) solid += counter.count; }
            return solid;
        }

        public static float GetRatio(ICollection<CwChangeCounter> counters = null)
        {
            return CwHelper.Divide(GetCount(counters), GetTotal(counters));
        }

        public static bool GetReady(ICollection<CwChangeCounter> counters = null)
        {
            foreach (var counter in counters ?? Instances)
                if (counter != null && counter.HasRead == false) return false;
            return true;
        }

        private void HandleCompleteChange(NativeArray<Color32> pixels)
        {
            if (changePixels.IsCreated && changePixels.Length != pixels.Length)
                changePixels.Dispose();

            if (!changePixels.IsCreated)
                changePixels = new NativeArray<Color32>(pixels.Length, Allocator.Persistent);

            NativeArray<Color32>.Copy(pixels, changePixels);

            HandleComplete(changeReader.DownsampleBoost);
        }

        protected override void HandleComplete(int boost)
        {
            if (!currentPixels.IsCreated || !maskPixels.IsCreated || !changePixels.IsCreated
                || currentPixels.Length != maskPixels.Length || currentPixels.Length != changePixels.Length)
                return;

#if UNITY_WEBGL
			// WebGL is single-threaded — run synchronously on main thread
			var threshold32 = (byte)(threshold * 255.0f);
			var oldTotal    = total;
			count = 0;
			total = 0;

			for (var i = 0; i < currentPixels.Length; i++)
			{
				if (maskPixels[i] > 127)
				{
					total++;
					var cur = currentPixels[i];
					var chg = changePixels[i];
					int distance = 0;
					distance += System.Math.Abs(chg.r - cur.r);
					distance += System.Math.Abs(chg.g - cur.g);
					distance += System.Math.Abs(chg.b - cur.b);
					distance += System.Math.Abs(chg.a - cur.a);
					if (distance > threshold32) count++;
				}
			}

			total *= boost;
			count *= boost;

			if (CalculateTotal == false) total = oldTotal;
			InvokeOnUpdated();
#else
            // Non-WebGL — schedule Burst job off main thread
            FlushPendingJob();

            if (!_jobResults.IsCreated)
                _jobResults = new NativeArray<int>(2, Allocator.Persistent);

            _jobResults[0] = 0;
            _jobResults[1] = 0;
            _pendingBoost = boost;

            var job = new CountChangedPixelsJob
            {
                CurrentPixels = currentPixels,
                MaskPixels = maskPixels,
                ChangePixels = changePixels,
                Threshold32 = (byte)(threshold * 255.0f),
                Results = _jobResults,
            };

            _jobHandle = job.Schedule();
            _jobPending = true;
            // Results applied in Update() when job completes
#endif
        }

        protected override void Update()
        {
            base.Update();

#if !UNITY_WEBGL
            // Poll job completion each frame — apply results when ready
            if (_jobPending && _jobHandle.IsCompleted)
            {
                _jobHandle.Complete();
                _jobPending = false;

                var oldTotal = total;
                count = _jobResults[0] * _pendingBoost;
                total = _jobResults[1] * _pendingBoost;

                if (CalculateTotal == false)
                    total = oldTotal;

                InvokeOnUpdated();
            }
#endif

            // CwChangeCounter-specific update (change reader request)
            if (changeReader.Requested == false && registeredPaintableTexture != null && registeredPaintableTexture.Activated == true)
            {
                if (CwReader.NeedsUpdating(changeReader, changePixels, registeredPaintableTexture.Current, downsampleSteps))
                {
                    var desc = registeredPaintableTexture.Current.descriptor; desc.useMipMap = false;
                    var renderTexture = CwCommon.GetRenderTexture(desc);

                    CwCommandReplace.Blit(renderTexture, texture, color);

                    changeReader.Request(renderTexture, DownsampleSteps, Async);

                    CwCommon.ReleaseRenderTexture(renderTexture);
                }
            }
        }

#if !UNITY_WEBGL
        private void FlushPendingJob()
        {
            if (_jobPending)
            {
                _jobHandle.Complete();
                _jobPending = false;
            }
        }
#endif

        protected override void OnEnable()
        {
            instancesNode = Instances.AddLast(this);
            base.OnEnable();

            if (changeReader == null)
            {
                changeReader = new CwReader();
                changeReader.OnComplete += HandleCompleteChange;
            }
        }

        protected override void OnDisable()
        {
            Instances.Remove(instancesNode); instancesNode = null;
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
#if !UNITY_WEBGL
            FlushPendingJob();
#endif
            base.OnDestroy();

            if (changeReader != null)
            {
                changeReader.OnComplete -= HandleCompleteChange;
                changeReader.Release();
            }

            if (changePixels.IsCreated) changePixels.Dispose();
#if !UNITY_WEBGL
            if (_jobResults.IsCreated) _jobResults.Dispose();
#endif
        }

        protected override void Start()
        {
            base.Start();
            if (MaskReader.Dirty == true)
                changeReader.MarkAsDirty();
        }
    }
}

#if UNITY_EDITOR
namespace PaintCore
{
    using UnityEditor;
    using TARGET = CwChangeCounter;

    [CustomEditor(typeof(TARGET))]
    public class CwChangeCounter_Editor : CwPaintableTextureMonitorMask_Editor
    {
        protected override void OnInspector()
        {
            TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

            var markAsDirty = false;

            base.OnInspector();

            Separator();

            Draw("threshold", ref markAsDirty, "The RGBA value must be higher than this for it to be counted.");
            DrawTexture(tgts, markAsDirty);
            DrawColor(tgts, markAsDirty);

            Separator();

            Draw("total");

            EditorGUILayout.BeginHorizontal();
            Draw("count");
            EditorGUI.ProgressBar(Reserve(), tgt.Ratio, "Ratio");
            EditorGUILayout.EndHorizontal();

            if (markAsDirty == true)
                Each(tgts, t => t.MarkChangeReaderAsDirty(), true);
        }

        private void DrawTexture(TARGET[] tgts, bool dirtyChange)
        {
            EditorGUILayout.BeginHorizontal();
            Draw("texture", ref dirtyChange, "The texture we want to compare change to.\n\nNone/null = white.\n\nNOTE: All pixels in this texture will be tinted by the current Color.");
            BeginDisabled(All(tgts, t => t.PaintableTexture == null || t.PaintableTexture.Texture == t.Texture));
            if (GUILayout.Button("Copy", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) == true)
            {
                Undo.RecordObjects(targets, "Copy Texture"); Each(tgts, t => { if (t.PaintableTexture != null) { t.Texture = t.PaintableTexture.Texture; EditorUtility.SetDirty(t); } });
            }
            EndDisabled();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawColor(TARGET[] tgts, bool dirtyChange)
        {
            EditorGUILayout.BeginHorizontal();
            Draw("color", ref dirtyChange, "The color we want to compare change to.\n\nNOTE: All pixels in the Texture will be tinted by this.");
            BeginDisabled(All(tgts, t => t.PaintableTexture == null || t.PaintableTexture.Color == t.Color));
            if (GUILayout.Button("Copy", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) == true)
            {
                Undo.RecordObjects(targets, "Copy Color"); Each(tgts, t => { if (t.PaintableTexture != null) { t.Color = t.PaintableTexture.Color; EditorUtility.SetDirty(t); } });
            }
            EndDisabled();
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif