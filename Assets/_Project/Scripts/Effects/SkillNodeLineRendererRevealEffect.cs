using System.Collections;
using System.Collections.Generic;
using inkolorgames.effects;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SkillNode))]
public class SkillNodeLineRendererRevealEffect : Effect
{
    [Header("Animation Settings")]
    [SerializeField, Range(0f,2f)] private float lineRevealDuration = 0.5f;
    [SerializeField] private AnimationCurve lineRevealCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private UnityEvent OnComplete;

    [Header("Reveal Direction")]
    [SerializeField] private bool reverseReveal = false;

    private Dictionary<LineRenderer, Vector3[]> originalLinePositions = new Dictionary<LineRenderer, Vector3[]>();
    private List<LineRenderer> connectionLines;
    private SkillNode skillNode;
    private Coroutine coroutine;

    private void Awake()
    {
        skillNode = GetComponent<SkillNode>();
        connectionLines = skillNode.ConnectionLines;

        foreach (var line in connectionLines)
        {
            if (line != null)
            {
                Vector3[] positions = new Vector3[line.positionCount];
                line.GetPositions(positions);
                originalLinePositions[line] = positions;
            }
        }
    }

    public override void DoEffect()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(AnimateLineReveal());
    }

    private IEnumerator AnimateLineReveal()
    {
        Vector3 startPos, endPos;

        foreach (var line in connectionLines)
        {
            if (line != null && originalLinePositions.ContainsKey(line))
            {
                line.gameObject.SetActive(true);
                Vector3[] positions = originalLinePositions[line];

                startPos = positions[0];
                endPos = positions[1];

                if (reverseReveal)
                {
                    line.SetPosition(0, endPos);
                    line.SetPosition(1, endPos);
                }
                else
                {
                    line.SetPosition(0, startPos);
                    line.SetPosition(1, startPos);
                }
            }
        }

        float elapsed = 0f;
        while (elapsed < lineRevealDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / lineRevealDuration);
            float curvedT = lineRevealCurve.Evaluate(t);

            foreach (var line in connectionLines)
            {
                if (line != null && originalLinePositions.ContainsKey(line))
                {
                    Vector3[] originalPositions = originalLinePositions[line];
                    startPos = originalPositions[0];
                    endPos = originalPositions[1];

                    if (reverseReveal)
                    {
                        Vector3 currentStartPos = Vector3.Lerp(endPos, startPos, curvedT);
                        line.SetPosition(0, currentStartPos);
                        line.SetPosition(1, endPos);
                    }
                    else
                    {
                        Vector3 currentEndPos = Vector3.Lerp(startPos, endPos, curvedT);
                        line.SetPosition(0, startPos);
                        line.SetPosition(1, currentEndPos);
                    }
                }

                yield return null;
            }
        }

        foreach (var line in connectionLines)
        {
            if (line != null && originalLinePositions.ContainsKey(line))
            {
                Vector3[] positions = originalLinePositions[line];
                line.SetPosition(0, positions[0]);
                line.SetPosition(1, positions[1]);
            }
        }

        OnComplete?.Invoke();
    }
}
