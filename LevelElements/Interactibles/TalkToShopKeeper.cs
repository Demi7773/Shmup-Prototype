using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkToShopKeeper : Interactible
{

    [SerializeField] private GameObject _canvas;
    [SerializeField] private Image _bubble;
    [SerializeField] private TextMeshProUGUI _bubbleText;

    [SerializeField] private Camera _cam;
    [SerializeField] private bool _canTalk = true;
    [SerializeField] private float _displayDuration = 10.0f;
    [SerializeField] private float _timeBetweenTalks = 3.0f;
    [SerializeField] private List<string> _shopKeeperRandomResponses = new List<string>();
    [SerializeField] private string _lowHPResponse = "You look really messed up! You should take a sip from the fountain over there. It'll fix ya right up!";




    private void Start()
    {
        _cam = Camera.main;
    }


    public override void Interact(PlayerReferences playerRef)
    {


        if (_cam == null)
        {
            _cam = Camera.main;
        }
        if (!_canTalk)
            return;

        string response = "";

        //if (GameManager.Instance.)
        //{
        //    response = ;
        //    return;
        //}
        if (playerRef.PlayerHP.CurrentHP < 15.0f)
        {
            response = _lowHPResponse;
            DisplayMySpeechBubble(response);
            return;
        }
        
        int rand = Random.Range(0, _shopKeeperRandomResponses.Count);
        response = _shopKeeperRandomResponses[rand];
        DisplayMySpeechBubble(response);
    }


    private void DisplayMySpeechBubble(string text)
    {
        _bubbleText.text = text;
        StartCoroutine(BubbleSequence());
    }
    private IEnumerator BubbleSequence()
    {
        _canTalk = false;
        _bubble.gameObject.SetActive(true);
        yield return new WaitForSeconds(_displayDuration);
        _bubble.gameObject.SetActive(false);
        yield return new WaitForSeconds(_timeBetweenTalks);
        _canTalk = true;
    }



    private void Update()
    {
        _canvas.transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position, _cam.transform.up);
    }


}
