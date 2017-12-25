using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using VRTK;

public class PostProcessingController : MonoBehaviour
{

    public enum EffectList { empty, dead, hit }

    [SerializeField] PostProcessingProfile deadPostProcessing;
    [SerializeField] PostProcessingProfile hitPostProcessing;

    public static PostProcessingController Instance { get; private set; }

    private EffectList _cameraEffect;
    public EffectList CameraEffect
    {
        get { return _cameraEffect; }
        private set
        {
            if (value == _cameraEffect)
                return;

            if (!inited)
                this.DoAtNextFrame(() => CameraEffect = value);

            if (_cameraEffect == EffectList.empty)
                cameraBehaviour.enabled = true;

            switch (value)
            {
                case EffectList.empty:
                    cameraBehaviour.enabled = false;
                    break;
                case EffectList.dead:
                    cameraBehaviour.profile = deadPostProcessing;
                    break;
                case EffectList.hit:
                    cameraBehaviour.profile = hitPostProcessing;
                    break;
            }

            _cameraEffect = value;

        }
    }

    PostProcessingBehaviour cameraBehaviour;
    private bool inited = false;
    private Coroutine effectStopCoroutine;
    EffectList mainEffect;

    private void Awake()
    {
        if (Instance != null)
            DestroyImmediate(this);
        Instance = this;
    }

    void Start()
    {
        // At first frame it will have error;
        this.DoAtNextFrame(() =>
        {
            inited = true;

            Transform camera = VRTK_DeviceFinder.HeadsetCamera();
            cameraBehaviour = camera.GetComponent<PostProcessingBehaviour>();
            if (cameraBehaviour == null)
            {
                cameraBehaviour = camera.gameObject.AddComponent<PostProcessingBehaviour>();
                cameraBehaviour.enabled = false;
            }

        });
    }

    public void SetEffect(int effectID, float deactivationTime = 0)
    {
        SetEffect((EffectList)effectID, deactivationTime);
    }

    public void SetEffect(EffectList effect, float deactivationTime = 0)
    {

        // if should change to old effect stop it
        if (effectStopCoroutine != null)
        {
            CameraEffect = mainEffect;
            StopCoroutine(effectStopCoroutine);
            effectStopCoroutine = null;
        }

        //if effect not temp
        if (deactivationTime > 0)
            effectStopCoroutine = StartCoroutine(this.DoWithDelayCoroutine(() =>
            {
                CameraEffect = mainEffect;
                effectStopCoroutine = null;
            }, deactivationTime));
        else
            mainEffect = effect;
        CameraEffect = effect;
    }
}
