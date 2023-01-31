using DG.Tweening;
using UnityEngine;

namespace Pilk.Scripts
{
    public class JumpPlatform : MonoBehaviour
    {
        [SerializeField] private float _jumpForce;
        [SerializeField] private Collider _collider;
        [SerializeField] private float _dissolveDuration;
        private MeshRenderer _meshRenderer;
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");

        private void Start()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
    
        private void OnTriggerEnter(Collider other)
        {
            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(
                    transform.DOMove(transform.position + Vector3.up * .3f, .3f / _jumpForce * Time.deltaTime)
                )
                .AppendCallback(
                    () =>
                    {
                        var characterController = other.gameObject.GetComponent<PlatformingCharacterController>();
                        characterController.SetVerticalVelocity(_jumpForce);
                    }
                )
                .AppendCallback(() => _collider.enabled = false)
                .Append(
                    DOTween.To(value =>
                        {
                            _meshRenderer.material.SetFloat(Dissolve, value);
                        },
                        0f, 1f, _dissolveDuration)
                )
                .AppendCallback(() => gameObject.SetActive(false));
        }
    }
}
