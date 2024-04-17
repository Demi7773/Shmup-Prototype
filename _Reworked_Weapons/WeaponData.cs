using UnityEngine;

namespace ReworkedWeapons
{
    [CreateAssetMenu(fileName = "WeaponData")]
    public class WeaponData : ScriptableObject
    {

        [SerializeField] private Weapon _weaponInstance;
        [SerializeField] private int _price;

        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private AudioClip _fireAudio;
        [SerializeField] private AudioClip _emptyClipAudio;
        [SerializeField] private AudioClip _reloadStartAudio;
        [SerializeField] private AudioClip _reloadedAudio;
        public Weapon WeaponInstance => _weaponInstance;

        public int Price => _price;
        public string Name => _name;
        public Sprite Sprite => _sprite;
        public AudioClip FireAudio => _fireAudio;
        public AudioClip EmptyClipAudio => _emptyClipAudio;
        public AudioClip ReloadStartAudio => _reloadStartAudio;
        public AudioClip ReloadedAudio => _reloadedAudio;

    }
}