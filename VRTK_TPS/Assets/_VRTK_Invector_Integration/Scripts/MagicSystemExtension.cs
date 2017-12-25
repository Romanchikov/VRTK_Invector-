using System.Collections;
using UnityEngine;

public delegate void VoidDelegate();

public static class Extension
{
    public static void DoWithDelay(this MonoBehaviour mono, VoidDelegate func, float delay)
    {
        mono.StartCoroutine(DoWithDelayCoroutine(mono, func, delay));
    }

    public static void DoAtEndOfFrame(this MonoBehaviour mono, VoidDelegate func)
    {
        mono.StartCoroutine(DoAtEndOfFrameCoroutine(mono, func));
    }

    public static void DoAtNextFrame(this MonoBehaviour mono, VoidDelegate func)
    {
        mono.StartCoroutine(DoAtNextFrameCoroutine(mono, func));
    }

    


    public static IEnumerator DoWithDelayCoroutine(this MonoBehaviour mono, VoidDelegate func, float delay)
	{
        yield return new WaitForSeconds(delay);
        func();
	}

	public static IEnumerator DoAtEndOfFrameCoroutine(this MonoBehaviour mono, VoidDelegate func)
	{
        yield return new WaitForEndOfFrame();
		func();
	}

	public static IEnumerator DoAtNextFrameCoroutine(this MonoBehaviour mono, VoidDelegate func)
	{
		yield return null;
		func();
	}

}

		
