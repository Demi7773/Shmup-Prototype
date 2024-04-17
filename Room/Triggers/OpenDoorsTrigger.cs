using UnityEngine;

public class OpenDoorsTrigger : MonoBehaviour
{

    [SerializeField] private Door _door;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _door.SetOpened(true);
        }
    }

}
