﻿using UnityEngine;
using UnityEngine.UI;

namespace Koffie.SimpleTasks.Examples
{
    public class STaskCooldownUI : MonoBehaviour
    {
        [SerializeField]
        private Image _donutImage;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetProgress(float progress)
        {
            _donutImage.fillAmount = progress;
        }
    }
}