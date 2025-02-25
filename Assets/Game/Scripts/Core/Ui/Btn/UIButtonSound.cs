using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ui.Btn
{
    public class UIButtonSound : UIBaseButton
    {
        public string SFX_Address = AddressableName.SFX_Button_Common;
        [Range(0, 1)]
        public float volumn = 1;

        public override async void Action()
        {
            Logger.Log("Action Sound: " + gameObject.name);
            await GameSceneManager.Instance.RequestAsync<AudioClip>(SFX_Address, SFX_Address);

            var audio = GameSceneManager.Instance.GetAsset<AudioClip>(SFX_Address);
            // Play Audio Here
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }
}
