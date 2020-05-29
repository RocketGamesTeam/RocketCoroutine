using System;
using System.Collections;
using UnityEngine;

namespace RocketCoroutine.Demo
{
    public class RocketCoroutineDemo : MonoBehaviour {

        public void Start()
        {
            //Start Coroutine WITHOUT key
            RoutineWithoutKey().StartCoroutine();
            //CoroutineController.StartCoroutine(RoutineWithKey()); <--- This also can be used

            //Do After Given Time with ANONYMOUS action
            CoroutineController.DoAfterGivenTime(3f, () =>
            {
                Debug.Log("OVERRIDE!");
                //Override Coroutine WITHOUT key
                RoutineWithoutKey().StartCoroutine(true);

                CoroutineController.DoAfterGivenTime(3f, () =>
                {
                    //Stop Coroutine WITHOUT key
                    Debug.Log("STOP!");
                    RoutineWithoutKey().StopCoroutine();
                });
            });

            //Cancellable Do After
            CoroutineController.DoAfterGivenTime(10f, () =>
            {
                Debug.LogError("This should NOT be shown!");
            }, "killme");

            CoroutineController.DoAfterGivenTime(9f, () =>
            {
                CoroutineController.StopCoroutine("killme");
            });

            //Do After Given Time with action
            Action action = () =>
            {
                //Start Coroutine WITH key
                RoutineWithKey().StartCoroutine("myKey");
                CoroutineController.DoAfterGivenTime(3f, () =>
                {
                    //Stop Coroutine WITH KEY
                    RoutineWithKey().StopCoroutine("myKey");
                });
            };

            action.DoAfterGivenTime(12f);
            CoroutineController.DoAfterGivenTime(18f, () =>
            {
                RoutineWillDieItSelf().StartCoroutine(onFinished: isStopped =>
                {
                    Debug.Log("RoutineWillDieItSelf died by itself. IsStopped: " + isStopped);
                });
            });
        }
        
        public IEnumerator RoutineWithoutKey()
        {
            int i = 0;
            while (true)
            {
                yield return new WaitForSeconds(1f);
                i++;
                Debug.Log("W/O Key #" + i);
            }
        }

        public IEnumerator RoutineWithKey()
        {
            int i = 0;
            while (true)
            {
                yield return new WaitForSeconds(1f);
                i++;
                Debug.Log("W Key #" + i);
            }
        }

        public IEnumerator RoutineWillDieItSelf()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}
