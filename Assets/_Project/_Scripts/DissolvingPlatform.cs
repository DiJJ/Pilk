using DG.Tweening;
using UnityEngine;

namespace Pilk.Scripts
{
    public class DissolvingPlatform : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private float _sustainDuration;
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
                .AppendInterval(_sustainDuration)
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
