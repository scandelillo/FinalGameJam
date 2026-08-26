using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light2D light2D;

    [Header("Tiempo entre fallos")]
    [SerializeField] private float minTimeBetweenFlickers = 3f;
    [SerializeField] private float maxTimeBetweenFlickers = 8f;

    [Header("Parpadeo")]
    [SerializeField] private int minFlickers = 2;
    [SerializeField] private int maxFlickers = 5;

    [SerializeField] private float minFlickerDuration = 0.03f;
    [SerializeField] private float maxFlickerDuration = 0.09f;

    [Header("Intensidad durante fallo")]
    [Range(0f, 1f)]
    [SerializeField] private float minIntensityMultiplier = 0.05f;

    [Range(0f, 1f)]
    [SerializeField] private float maxIntensityMultiplier = 0.35f;

    private float originalIntensity;

    private void Awake()
    {
        if (light2D == null)
        {
            light2D = GetComponent<Light2D>();
        }

        if (light2D != null)
        {
            originalIntensity = light2D.intensity;
        }
    }

    private void Start()
    {
        if (light2D != null)
        {
            StartCoroutine(FlickerRoutine());
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(
                minTimeBetweenFlickers,
                maxTimeBetweenFlickers
            );

            yield return new WaitForSeconds(waitTime);

            int flickerCount = Random.Range(
                minFlickers,
                maxFlickers + 1
            );

            for (int i = 0; i < flickerCount; i++)
            {
                float intensityMultiplier = Random.Range(
                    minIntensityMultiplier,
                    maxIntensityMultiplier
                );

                light2D.intensity =
                    originalIntensity * intensityMultiplier;

                float flickerDuration = Random.Range(
                    minFlickerDuration,
                    maxFlickerDuration
                );

                yield return new WaitForSeconds(flickerDuration);

                light2D.intensity = originalIntensity;

                float normalDuration = Random.Range(
                    0.03f,
                    0.12f
                );

                yield return new WaitForSeconds(normalDuration);
            }

            light2D.intensity = originalIntensity;
        }
    }
}