using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyReward : MonoBehaviour
{
    public bool initialized; //true ise sınıf tanımlanmıştır.
    public long rewardGivingTimeTick;
    [SerializeField] GameObject rewardMenu;
    [SerializeField] TMP_Text remainingTimeText;


    public void InitilalizeDailyReward()
    {
        if (PlayerPrefs.HasKey("lastDailyReward"))
        {
            rewardGivingTimeTick = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;
            long currentTime = System.DateTime.Now.Ticks;
            if (currentTime >= rewardGivingTimeTick)
            {
                GiveReward();
            }
        }
        else
        {
            GiveReward(); //daha önce ödül almamışsa.
        }

        initialized = true;
    }


    public void GiveReward()
    {
        LevelController.Current.GiveGoldToPlayer(100);
        rewardMenu.SetActive(true);
        PlayerPrefs.SetString("lastDailyReward", System.DateTime.Now.Ticks.ToString()); //son ödül alım tarihi.
        rewardGivingTimeTick = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000; //bir sonraki ödül alım.
    }



    void Update()
    {
        if (initialized)
        {
            if (LevelController.Current.startMenu.activeInHierarchy)
            {
                long currentTime = System.DateTime.Now.Ticks;
                long remainingTime = rewardGivingTimeTick - currentTime;
                if (remainingTime <= 0)
                {
                    GiveReward();
                }
                else
                {
                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remainingTime);
                    remainingTimeText.text = string.Format("{0}:{1}:{2}", timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));

                }
            }
        }
    }

    public void TapToReturn()
    {
        rewardMenu.SetActive(false);
    }

}

