using System.Collections;
using UnityEngine;

namespace Architecture.Boot{
	public interface ICoroutineRunner
	{
		Coroutine StartCoroutine(IEnumerator load);
	}
}