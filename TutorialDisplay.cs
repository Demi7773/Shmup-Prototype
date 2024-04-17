using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class TutorialDisplay : MonoBehaviour
{

    [SerializeField] private float _displayDuration = 5.0f;

    [SerializeField] private RectTransform _tutorial1;
    [SerializeField] private RectTransform _tutorial2;
    //[SerializeField] private Image _bgImage;
    //[SerializeField] private TextMeshProUGUI _messageTxt;
    //[SerializeField] private TextMeshProUGUI _onPanelImage;

    //[SerializeField] private string _tutorial1Tx



    public enum TutorialContent
    {
        Tutorial1,
        Tutorial2
    }




    private void OnEnable()
    {
        UIEvents.OnDisplayNewTutorial += DisplayTutorialForLevel;
    }
    private void OnDisable()
    {
        UIEvents.OnDisplayNewTutorial -= DisplayTutorialForLevel;
    }

    private void DisplayTutorialForLevel(TutorialContent content)
    {
        switch (content)
        {
            case TutorialContent.Tutorial1:
                StartCoroutine(StartDisplaySequence(_tutorial1));
                break;
            case TutorialContent.Tutorial2:
                StartCoroutine(StartDisplaySequence(_tutorial2));
                break;
        }
    }



    private IEnumerator StartDisplaySequence(RectTransform tutorial)
    {
        yield return new WaitForSeconds(1f);
        //Debug.Log("Displaying tutorial " + tutorial.name);
        tutorial.gameObject.SetActive(true);
        yield return new WaitForSeconds(_displayDuration);
        tutorial.gameObject.SetActive(false);
    }

}
