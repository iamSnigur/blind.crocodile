using System.Collections;
using UnityEngine;

namespace Scripts.Core
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator routine);
    }
}