using UnityEngine;

public class PlayerSensors : MonoBehaviour
{

    [SerializeField] private PlayerMoney _money;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pickUpMoneySound;
    //[SerializeField] private float _pickUpMoneyVolume = 1.0f;

    [SerializeField] private LayerMask _interactiblesLayer;
    [SerializeField] private float _interactiblesRange = 1.0f;
    [SerializeField] private Interactible _closestInteractible = null;

    [SerializeField] private LayerMask _moneyDropsLayer;
    [SerializeField] private float _moneyStartPickUpRange = 3.0f;
    [SerializeField] private float _getMoneyRange = 1.0f;

    //[SerializeField] private LayerMask _pickupsLayer;
    //[SerializeField] private float _pickupTriggerRange = 3.0f;
    //[SerializeField] private float _pickupCompleteRange = 1.0f;
    public Interactible ClosestInteractible => _closestInteractible;

    //[SerializeField] private LayerMask _groundLayers;
    //[SerializeField] private Transform[] _groundChecks = null;

  



    private void Update()
    {
        CollectMoney();
        CheckInteractibles();

        //CollectPickups();

        //GroundChecks();
        //WallChecks();
    }

    private void CollectMoney()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _moneyStartPickUpRange, _moneyDropsLayer);

        foreach (Collider hit in hits)
        {
            var pickup = hit.GetComponent<MoneyPickup>();
            if (pickup == null)
            {
                Debug.Log("Money Pickup null on scan");
                continue;
            }

            if (Vector3.Distance(transform.position, hit.transform.position) < _getMoneyRange)
            {
                int money = pickup.Amount;
                pickup.gameObject.SetActive(false);
                _money.GetMoney(money);

                //_audioSource.PlayOneShot(_pickUpMoneySound, _pickUpMoneyVolume);
                continue;
            }

            if (pickup.IsGettingPickedUp)
            {
                //Debug.Log("Is already getting picked up");
                continue;
            }

            pickup.StartPickupSequence(transform);
        }
    }
    private void CollectPickups()
    {
        //Collider[] hits = Physics.OverlapSphere(transform.position, _pickupTriggerRange, _pickupsLayer);

        //foreach (Collider hit in hits)
        //{
        //    var pickup = hit.GetComponent<PickUp>();
        //    if (pickup == null)
        //    {
        //        Debug.Log("Pickup null on scan");
        //        continue;
        //    }

        //    if (Vector3.Distance(transform.position, hit.transform.position) < _pickupCompleteRange)
        //    {

        //    }
        //}
    }

    private void CheckInteractibles()
    {

        if (_closestInteractible != null)
            if (Vector3.Distance(transform.position, _closestInteractible.transform.position) < _interactiblesRange)
                _closestInteractible = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, _interactiblesRange, _interactiblesLayer);
        if (hits.Length == 0)
        {
            _closestInteractible = null;
            return;
        }

        float compareDistance = _interactiblesRange + 1;
        Collider closest = null;
        foreach (Collider hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.ClosestPoint(transform.position));
            if (distance < compareDistance)
            {
                closest = hit;
                compareDistance = distance;
            }
        }

        if (closest == null)
            return;

        Interactible interactible = closest.GetComponent<Interactible>();
        if (interactible == null)
            return;

        _closestInteractible = interactible;
    }

}
