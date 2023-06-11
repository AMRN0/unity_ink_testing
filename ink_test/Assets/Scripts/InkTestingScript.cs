using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;

public class InkTestingScript : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private TMP_Text TMPtext;
    [SerializeField] private Button button;

    private Story story;

    private void Awake()
    {
        story = new Story(inkJSON.text);
    }    

    // Start is called before the first frame update
    void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        EraseUI();

        // Read all the content until we can't continue any more
        while (story.canContinue)
        {
            // Continue gets the next line of the story
            string text = story.Continue();
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            //CreateContentView(text);
            TMP_Text storyText = Instantiate(TMPtext) as TMP_Text;
            storyText.text = text;
            storyText.transform.SetParent(this.transform, false);
        }

        // Display all the choices, if there are any!
        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                //Button button = CreateChoiceView(choice.text.Trim());
                Button cButton = Instantiate(button) as Button;
                cButton.transform.SetParent(this.transform, false);

                // Gets the text from the button prefab
                TMP_Text choiceText = cButton.GetComponentInChildren<TMP_Text>();
                choiceText.text = choice.text.Trim();
                // Tell the button what to do when we press it
                cButton.onClick.AddListener(delegate {
                    ChooseStoryChoice(choice);
                });
            }
        }
        // If we've read all the content and there's no choices, the story is finished!
        else
        {
            Button choice = Instantiate(button) as Button;
            choice.transform.SetParent(this.transform, false);

            // Gets the text from the button prefab
            TMP_Text choiceText = choice.GetComponentInChildren<TMP_Text>();
            choiceText.text = "End of story.\nRestart?";
            choice.onClick.AddListener(delegate
            {
                story.ResetState();
                RefreshUI();
            });
        }
    }

    void EraseUI()
    {
        int childCount = this.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    void ChooseStoryChoice(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshUI();
    }
}
