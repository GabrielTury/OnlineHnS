using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUDManager : NetworkBehaviour
{
    [System.Serializable]
    public class HUDType
    {
        public string name;
        public Sprite avatar;
        public float hp;
        public List<HUDSkill> skills;
    }

    [System.Serializable]
    public class HUDSkill
    {
        public Sprite icon;
    }

    [SerializeField]
    private HUDType[] playerSlots;

    [System.Serializable]
    public class HUDSlot
    {
        public TextMeshProUGUI nameHUD;
        public Image avatarHUD;
        public SlicedFilledImage hpHUD;
        public SlicedFilledImage hpBackgroundHUD;
        public List<Image> skillHUD;
        public List<TextMeshProUGUI> skillCooldownHUD;
    }

    [SerializeField]
    private HUDSlot player1Slot;

    [SerializeField]
    private HUDSlot player2Slot;

    private Coroutine[] skillP1Coroutines = new Coroutine[5];
    private Coroutine[] skillP2Coroutines = new Coroutine[5];


    //private HUDType player1HUD;

    private void OnEnable()
    {
        GameEvents.Player_Damaged += UpdateHP;
    }

    private void OnDisable()
    {
        GameEvents.Player_Damaged -= UpdateHP;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Change the index 0 to actual desired player selection
        player1Slot.nameHUD.text = playerSlots[0].name;
        player1Slot.avatarHUD.sprite = playerSlots[0].avatar;
        player1Slot.hpHUD.fillAmount = playerSlots[0].hp / 100;
        for (int i = 0; i <= 4; i++)
        {
            if (i < playerSlots[0].skills.Count)
            {
                player1Slot.skillHUD[i].sprite = playerSlots[0].skills[i].icon;
                player1Slot.skillCooldownHUD[i].enabled = false;
            }
            else
            {
                player1Slot.skillHUD[i].enabled = false;
                player1Slot.skillCooldownHUD[i].enabled = false;
            }
        }

        player2Slot.nameHUD.text = playerSlots[1].name;
        player2Slot.avatarHUD.sprite = playerSlots[1].avatar;
        player2Slot.hpHUD.fillAmount = playerSlots[1].hp / 100;
        for (int i = 0; i <= 4; i++)
        {
            if (i < playerSlots[1].skills.Count)
            {
                player2Slot.skillHUD[i].sprite = playerSlots[1].skills[i].icon;
                player2Slot.skillCooldownHUD[i].enabled = false;
            }
            else
            {
                player2Slot.skillHUD[i].enabled = false;
                player2Slot.skillCooldownHUD[i].enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetupPlayerSlots()
    {
        if (IsServer)
        {
            player1Slot.nameHUD.text = playerSlots[0].name;
            player1Slot.avatarHUD.sprite = playerSlots[0].avatar;
            player1Slot.hpHUD.fillAmount = playerSlots[0].hp / 100;
            for (int i = 0; i <= 4; i++)
            {
                if (i < playerSlots[0].skills.Count)
                {
                    player1Slot.skillHUD[i].sprite = playerSlots[0].skills[i].icon;
                    player1Slot.skillCooldownHUD[i].enabled = false;
                }
                else
                {
                    player1Slot.skillHUD[i].enabled = false;
                    player1Slot.skillCooldownHUD[i].enabled = false;
                }
            }

            player2Slot.nameHUD.text = playerSlots[1].name;
            player2Slot.avatarHUD.sprite = playerSlots[1].avatar;
            player2Slot.hpHUD.fillAmount = playerSlots[1].hp / 100;
            for (int i = 0; i <= 4; i++)
            {
                if (i < playerSlots[1].skills.Count)
                {
                    player2Slot.skillHUD[i].sprite = playerSlots[1].skills[i].icon;
                    player2Slot.skillCooldownHUD[i].enabled = false;
                }
                else
                {
                    player2Slot.skillHUD[i].enabled = false;
                    player2Slot.skillCooldownHUD[i].enabled = false;
                }
            }
        }
        else
        {
            player1Slot.nameHUD.text = playerSlots[1].name;
            player1Slot.avatarHUD.sprite = playerSlots[1].avatar;
            player1Slot.hpHUD.fillAmount = playerSlots[1].hp / 100;
            for (int i = 0; i <= 4; i++)
            {
                if (i < playerSlots[0].skills.Count)
                {
                    player1Slot.skillHUD[i].sprite = playerSlots[1].skills[i].icon;
                    player1Slot.skillCooldownHUD[i].enabled = false;
                }
                else
                {
                    player1Slot.skillHUD[i].enabled = false;
                    player1Slot.skillCooldownHUD[i].enabled = false;
                }
            }

            player2Slot.nameHUD.text = playerSlots[0].name;
            player2Slot.avatarHUD.sprite = playerSlots[0].avatar;
            player2Slot.hpHUD.fillAmount = playerSlots[0].hp / 100;
            for (int i = 0; i <= 4; i++)
            {
                if (i < playerSlots[0].skills.Count)
                {
                    player2Slot.skillHUD[i].sprite = playerSlots[0].skills[i].icon;
                    player2Slot.skillCooldownHUD[i].enabled = false;
                }
                else
                {
                    player2Slot.skillHUD[i].enabled = false;
                    player2Slot.skillCooldownHUD[i].enabled = false;
                }
            }
        }
    }

    private void UpdateSkill(int playerNum, int skillNum)
    {
        if (playerNum == 1)
        {
            try { StopCoroutine(skillP1Coroutines[skillNum]); } catch { }
            player1Slot.skillHUD[skillNum].fillAmount = 0;
            skillP1Coroutines[skillNum] = StartCoroutine(UIUtils.FillOverSecondsImage(player1Slot.skillHUD[skillNum], 1, 1, player1Slot.skillCooldownHUD[skillNum]));
        }
        else if (playerNum == 2)
        {
            try { StopCoroutine(skillP2Coroutines[skillNum]); } catch { }
            player2Slot.skillHUD[skillNum].fillAmount = 0;
            skillP2Coroutines[skillNum] = StartCoroutine(UIUtils.FillOverSecondsImage(player2Slot.skillHUD[skillNum], 1, 1, player2Slot.skillCooldownHUD[skillNum]));
        }
        
    }

    void UpdateHP(float damage, int playerIndex)
    {
        if (IsServer)
        {
            if (playerIndex == 0)
            {
                playerSlots[0].hp -= damage;
                player1Slot.hpHUD.fillAmount = playerSlots[0].hp / 100;
            }
            else if (playerIndex == 1)
            {
                playerSlots[1].hp -= damage;
                player2Slot.hpHUD.fillAmount = playerSlots[1].hp / 100;
            }
        }
        else
        {
            if (playerIndex == 0)
            {
                playerSlots[1].hp -= damage;
                player1Slot.hpHUD.fillAmount = playerSlots[1].hp / 100;
            }
            else if (playerIndex == 1)
            {
                playerSlots[0].hp -= damage;
                player2Slot.hpHUD.fillAmount = playerSlots[0].hp / 100;
            }
        }
        

        /*float test = playerSlots[playerIndex].hp;

        Color testColor;
        if (test < 60)
        {
            testColor = Color.green;
        }
        else if (test < 40)
        {
            testColor = Color.yellow;
        }
        else if (test < 20)
        {
            testColor = Color.red;
        } 
        else
        {
            testColor = Color.white;
        }

        player1Slot.hpHUD.color = testColor;
        player1Slot.hpBackgroundHUD.color = testColor;*/

        
        //player2Slot.hpHUD.fillAmount = playerSlots[1].hp / 100;
    }
}
