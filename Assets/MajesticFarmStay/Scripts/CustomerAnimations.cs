using System.Collections;
using UnityEngine;

namespace MergeIdle
{
    public class CustomerAnimations : MonoBehaviour
    {
        private Animator _customerAnimator;
        [Header("Idle Speed")]
        [SerializeField, Range(0.5f, 2f)] private float _minIdleSpeed = 0.5f;
        [SerializeField, Range(0.5f, 2f)] private float _maxIdleSpeed = 1.2f;

        [Header("Blinking speed")]
        [SerializeField, Range(0.1f, 0.8f)] private float _blinking = 0.3f;
        private float _blinkInterval;

        [Header("Change of animation states")]
        [SerializeField] private bool _randomizeState;
        [SerializeField] private float _currentIdleSpeed = 0.0f;
        [SerializeField, Range(1f, 5f)] private float _minimumStateChangeTime = 5f;
        [SerializeField, Range(5f, 15f)] private float _maximumStateChangeTime = 15f;
        private float stateChangeWaitingTime = 0.0f;

        
        private void OnEnable()
        {
            _customerAnimator = GetComponent<Animator>();
            if (_randomizeState)
            {
                RandomizeIdleSpeed();

                //Coroutines
                StartCoroutine(ChangeCostumerState());
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

//This is for testing purposes only, to be removed in the final version
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CloseEyes();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                LookLeft();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                LookRight();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                OpenMouth();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                CloseMouth();
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                EyebrowsUp();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                EyebrowsDown();
            }

        }
#endif

        private void RandomizeIdleSpeed()
        {
            float randomSpeed = Random.Range(_minIdleSpeed, _maxIdleSpeed);
            _currentIdleSpeed = randomSpeed;

            if (_customerAnimator.GetCurrentAnimatorStateInfo((int)CustomerAnimatorLayers.BaseLayer).shortNameHash == CustomerAnimatorAnimationHashes.Idle)
            {
                _customerAnimator.speed = randomSpeed;
            }
        }

        private void CloseEyes()
        {
            if (!_customerAnimator.isActiveAndEnabled)
                return;

            _customerAnimator.Play(CustomerAnimatorAnimationHashes.EyeLids_Closed);
            Invoke("OpenEyes", _blinking);
        }

        private void OpenEyes()
        {
            if (!_customerAnimator.isActiveAndEnabled)
                return;

            _customerAnimator.Play(CustomerAnimatorAnimationHashes.EyeLids_Open);
        }

        private void LookLeft()
        {
            if (!_customerAnimator.isActiveAndEnabled)
                return;

            _customerAnimator.Play(CustomerAnimatorAnimationHashes.Eyes_ToLeft);


            float animationLenght = _customerAnimator.GetCurrentAnimatorStateInfo((int)CustomerAnimatorLayers.Eyes_Movement).length;

            Invoke("EyesNeutral", animationLenght + 1f);
        }

        private void LookRight()
        {
            if (!_customerAnimator.isActiveAndEnabled)
                return;

            _customerAnimator.Play(CustomerAnimatorAnimationHashes.Eyes_ToRight);

            if(gameObject.name != "Customer_Helmy")
            {
                float animationLenght = _customerAnimator.GetCurrentAnimatorStateInfo((int)CustomerAnimatorLayers.Eyes_Movement).length;
                Invoke("EyesNeutral", animationLenght);   
            }

            
        }


        private void EyesNeutral()
        {
            if (!_customerAnimator.isActiveAndEnabled)
                return;
            CloseEyes();
            _customerAnimator.Play(CustomerAnimatorAnimationHashes.Eyes_Neutral);
        }

        public void OpenMouth()
        {
            if (!_customerAnimator.isActiveAndEnabled)
                return;
            _customerAnimator.Play(CustomerAnimatorAnimationHashes.Mouth_Open);
        }

        private void CloseMouth()
        {
            if (!_customerAnimator.isActiveAndEnabled)
                return;
            _customerAnimator.Play(CustomerAnimatorAnimationHashes.Mouth_Close);
        }

        private void EyebrowsNeutral()
        {
            if (!_customerAnimator.isActiveAndEnabled)
                return;
            _customerAnimator.Play(CustomerAnimatorAnimationHashes.Eyebrows_Neutral);
        }

        private void EyebrowsUp(bool requirementsComplete = false)
        {
            if (!_customerAnimator.isActiveAndEnabled)
                return;
            _customerAnimator.Play(CustomerAnimatorAnimationHashes.Eyebrows_Up);

            if (!requirementsComplete)
            {
                float animationLenght = _customerAnimator.GetCurrentAnimatorStateInfo((int)CustomerAnimatorLayers.Eyebrows).length;
                Invoke("EyebrowsDown", animationLenght);
            }
        }

        private void EyebrowsDown()
        {
            if (!_customerAnimator.isActiveAndEnabled)
                return;
            _customerAnimator.Play(CustomerAnimatorAnimationHashes.Eyebrows_Down);

            float animationLenght = _customerAnimator.GetCurrentAnimatorStateInfo((int)CustomerAnimatorLayers.Eyebrows).length;
            Invoke("EyebrowsNeutral", animationLenght);
        }

        

        public void SetCostumerState(CostumerMergingStates state)
        {
            switch (state)
            {
                case CostumerMergingStates.Idle:
                    _customerAnimator.Play(CustomerAnimatorAnimationHashes.Idle);
                    break;
                case CostumerMergingStates.Blinking:
                    CloseEyes();
                    break;
                case CostumerMergingStates.LookingLeft:
                    LookLeft();
                    break;
                case CostumerMergingStates.LookingRight:
                    LookRight();
                    break;
                case CostumerMergingStates.MouthOpen:
                    OpenMouth();
                    break;
                case CostumerMergingStates.MouthClose:
                    CloseMouth();
                    break; 
                default:
                    break;
            }
            StartCoroutine(ChangeCostumerState());
        }

        private IEnumerator ChangeCostumerState()
        {
            stateChangeWaitingTime = GetRandomFloat(_minimumStateChangeTime, _maximumStateChangeTime);
            yield return new WaitForSeconds(stateChangeWaitingTime);
            CostumerMergingStates animationState = GetRandomAnimationState();
            SetCostumerState(animationState);
        }
        private float GetRandomFloat(float minimumTime, float maximumTime)
        {
            return Random.Range(minimumTime, maximumTime);
        }

        private CostumerMergingStates GetRandomAnimationState()
        {
            System.Array values = System.Enum.GetValues(typeof(CostumerMergingStates));
            return (CostumerMergingStates)values.GetValue(Random.Range(0, values.Length));
        }

        
        
    }
}
