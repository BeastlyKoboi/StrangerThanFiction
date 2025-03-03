using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BindingAnim : MonoBehaviour
{
    private Binding binding;
    private GameObject splotchObj;
    private Image splotchImg;
    private RectTransform splotchRect;

    [Header("Animation Settings")]
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float rotationSpeedMin = 5.0f;
    [SerializeField] private float rotationSpeedMax = 10.0f;

    [Header("Pulse Settings")]
    [SerializeField] private float pulsePeriod = 1.5f;
    [SerializeField] private float pulsePeriodMin = 1.5f;
    [SerializeField] private float pulsePeriodMax = 0.5f;

    [Header("Scale Settings")]
    [SerializeField] private float scale = 1.0f;
    [SerializeField] private float scaleDeviation = 0.1f;
    [SerializeField] private float scaleMin = 1.0f;
    [SerializeField] private float scaleMax = 1.5f;

    [Header("Ink Shading")]
    [SerializeField] private RawImage inkShading;
    [SerializeField] private float shiftSpeed = 0.25f;
    [SerializeField] private float inkShadingAlpha = 0.0f;
    [SerializeField] private float inkShadingAlphaMin = 0.0f;
    [SerializeField] private float inkShadingAlphaMax = 0.9f;


    // Start is called before the first frame update
    void Start()
    {
        binding = GetComponent<Binding>();

        splotchObj = transform.Find("Splotch").gameObject;
        splotchImg = splotchObj.GetComponent<Image>();
        splotchRect = splotchObj.GetComponent<RectTransform>();

        binding.OnBindingChange.AddListener(UpdateAnimation);
        StartCoroutine(Pulse());
    }

    // Update is called once per frame
    void Update()
    {
        
        splotchObj.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

    }

    public IEnumerator Pulse()
    {
        yield return CoroutineUtils.Lerp(pulsePeriod / 2, (t) =>
        {
            splotchRect.localScale = Vector3.Lerp(
                new Vector3(scale - scaleDeviation, scale - scaleDeviation, scale - scaleDeviation), 
                new Vector3(scale + scaleDeviation, scale + scaleDeviation, scale + scaleDeviation), t);
        });
        yield return CoroutineUtils.Lerp(pulsePeriod / 2, (t) =>
        {
            splotchRect.localScale = Vector3.Lerp(
                new Vector3(scale + scaleDeviation, scale + scaleDeviation, scale + scaleDeviation), 
                new Vector3(scale - scaleDeviation, scale - scaleDeviation, scale - scaleDeviation), t);
        });

        StartCoroutine(Pulse());
    }

    public IEnumerator ChangeShading(float newValue)
    {
        float prevShaderAlpha = inkShadingAlpha;
        inkShadingAlpha = Mathf.Lerp(inkShadingAlphaMin, inkShadingAlphaMax, newValue);

        yield return CoroutineUtils.Lerp(shiftSpeed, (t) =>
        {
            inkShading.color = new Color(255, 255, 255, Mathf.Lerp(prevShaderAlpha, inkShadingAlpha, t));
        });
    }

    private UniTask UpdateAnimation(BindingState state)
    {
        // update the animation based on the binding state
        float damageDec = (float)state.currTotalBindingDamage / (float)state.bindingPower;

        rotationSpeed = Mathf.Lerp(rotationSpeedMin, rotationSpeedMax, damageDec);
        pulsePeriod = Mathf.Lerp(pulsePeriodMin, pulsePeriodMax, damageDec);
        scale = Mathf.Lerp(scaleMin, scaleMax, damageDec);

        StartCoroutine(ChangeShading(damageDec));


        return UniTask.CompletedTask;
    }
}
