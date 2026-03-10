using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/// <summary>
/// Effet de vague sur les lettres d'un TextMeshProUGUI.
/// Nécessite DOTween et TextMeshPro.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProWaveEffect : MonoBehaviour
{
    [Header("Références")]
    private TextMeshProUGUI _textMesh;

    [Header("Paramètres de la vague")]
    [Tooltip("Hauteur maximale du déplacement vertical (en pixels)")]
    public float waveAmplitude = 20f;

    [Tooltip("Décalage de phase entre chaque lettre (en secondes)")]
    public float waveFrequency = 0.1f;

    [Tooltip("Durée d'un cycle complet pour une lettre (montée + descente)")]
    public float waveDuration = 0.6f;

    [Tooltip("Type d'easing DOTween pour le mouvement")]
    public Ease easeType = Ease.InOutSine;

    [Header("Contrôle")]
    [Tooltip("Lancer l'effet automatiquement au démarrage")]
    public bool playOnStart = true;

    [Tooltip("Nombre de loops (-1 = infini)")]
    public int loopCount = -1;

    // --- Privé ---
    private TMP_MeshInfo[] _cachedMeshInfo;
    private Coroutine _waveCoroutine;
    private bool _isPlaying = false;
    private List<Tween> _activeTweens = new List<Tween>();

    // Offsets verticaux courants par vertex
    private float[] _vertexOffsets;

    // -------------------------------------------------------
    void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }


    void Start()
    {
        if (playOnStart)
            StartCoroutine(PlayNextFrame());
    }


    private IEnumerator PlayNextFrame()
    {
        // Attend la fin du frame pour que TMP ait fini de construire son mesh
        yield return new WaitForSecondsRealtime(0f);
        Play();
    }

    void OnDisable()
    {
        Stop();
    }

    // -------------------------------------------------------
    #region API Publique

    /// <summary>Démarre l'effet de vague.</summary>
    public void Play()
    {
        if (_isPlaying) Stop();
        _isPlaying = true;
        _waveCoroutine = StartCoroutine(WaveRoutine());
    }

    /// <summary>Arrête l'effet et remet les lettres en place.</summary>
    public void Stop()
    {
        _isPlaying = false;

        if (_waveCoroutine != null)
        {
            StopCoroutine(_waveCoroutine);
            _waveCoroutine = null;
        }

        KillAllTweens();
        ResetMesh();
    }

    /// <summary>Pause / reprend l'effet.</summary>
    public void TogglePause()
    {
        if (!_isPlaying) return;

        foreach (var t in _activeTweens)
        {
            if (t.IsActive())
            {
                if (t.IsPlaying()) t.Pause();
                else t.Play();
            }
        }
    }

    /// <summary>Applique les nouveaux paramètres à la volée et redémarre.</summary>
    public void ApplySettings(float amplitude, float frequency, float duration, Ease ease)
    {
        waveAmplitude = amplitude;
        waveFrequency = frequency;
        waveDuration = duration;
        easeType = ease;

        if (_isPlaying)
        {
            Stop();
            Play();
        }
    }

    #endregion

    // -------------------------------------------------------
    #region Logique interne

    private IEnumerator WaveRoutine()
    {
        // Attend la fin du frame pour garantir que le Canvas a rendu le texte
        yield return new WaitForSecondsRealtime(0f);

        // Force la mise à jour du mesh TMP
        _textMesh.ForceMeshUpdate();

        TMP_TextInfo textInfo = _textMesh.textInfo;
        int charCount = textInfo.characterCount;

        if (charCount == 0)
        {
            _isPlaying = false;
            yield break;
        }

        // Copie du mesh d'origine
        _cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        _vertexOffsets = new float[charCount];

        int loopsCompleted = 0;

        // Lance un tween par lettre
        for (int i = 0; i < charCount; i++)
        {
            int charIndex = i; // capture

            TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
            if (!charInfo.isVisible) continue;

            float delay = charIndex * waveFrequency;

            // Tween qui anime l'offset vertical de la lettre
            Tween t = DOTween
                .To(
                    () => _vertexOffsets[charIndex],
                    v => _vertexOffsets[charIndex] = v,
                    waveAmplitude,
                    waveDuration * 0.5f
                )
                .SetDelay(delay)
                .SetUpdate(true)
                .SetEase(easeType)
                .SetLoops(loopCount == -1 ? -1 : loopCount * 2, LoopType.Yoyo)
                .OnUpdate(() => ApplyOffsetToChar(charIndex));

            _activeTweens.Add(t);
        }

        // Boucle qui applique les modifications au mesh chaque frame
        while (_isPlaying)
        {
            _textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }
    }

    /// <summary>Déplace les 4 vertices d'un caractère selon son offset courant.</summary>
    private void ApplyOffsetToChar(int charIndex)
    {
        if (_textMesh == null) return;

        TMP_TextInfo textInfo = _textMesh.textInfo;
        if (charIndex >= textInfo.characterCount) return;

        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
        if (!charInfo.isVisible) return;

        int meshIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;

        // Vérifie que le tableau est assez grand
        Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;
        if (vertexIndex + 3 >= vertices.Length) return;

        // Récupère les positions d'origine depuis le cache
        Vector3[] cachedVerts = _cachedMeshInfo[meshIndex].vertices;
        if (cachedVerts == null || vertexIndex + 3 >= cachedVerts.Length) return;

        Vector3 offset = new Vector3(0f, _vertexOffsets[charIndex], 0f);

        vertices[vertexIndex + 0] = cachedVerts[vertexIndex + 0] + offset;
        vertices[vertexIndex + 1] = cachedVerts[vertexIndex + 1] + offset;
        vertices[vertexIndex + 2] = cachedVerts[vertexIndex + 2] + offset;
        vertices[vertexIndex + 3] = cachedVerts[vertexIndex + 3] + offset;
    }

    private void ResetMesh()
    {
        if (_textMesh == null || _cachedMeshInfo == null) return;

        TMP_TextInfo textInfo = _textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;
            Vector3[] cachedVerts = _cachedMeshInfo[meshIndex].vertices;

            if (cachedVerts == null || vertexIndex + 3 >= cachedVerts.Length) continue;
            if (vertexIndex + 3 >= vertices.Length) continue;

            vertices[vertexIndex + 0] = cachedVerts[vertexIndex + 0];
            vertices[vertexIndex + 1] = cachedVerts[vertexIndex + 1];
            vertices[vertexIndex + 2] = cachedVerts[vertexIndex + 2];
            vertices[vertexIndex + 3] = cachedVerts[vertexIndex + 3];
        }

        _textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    private void KillAllTweens()
    {
        foreach (var t in _activeTweens)
        {
            if (t != null && t.IsActive())
                t.Kill();
        }
        _activeTweens.Clear();
    }

    #endregion

    // -------------------------------------------------------
    #region Gizmos / Debug (Editor only)
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Redémarre en live si les paramètres changent en Play mode
        if (Application.isPlaying && _isPlaying)
        {
            Stop();
            Play();
        }
    }
#endif
    #endregion
}