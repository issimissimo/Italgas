﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

namespace Michsky.UI.ModernUIPack
{
    public class ModalWindowManager : MonoBehaviour
    {
        // Resources
        public Image windowIcon;
        public TextMeshProUGUI windowTitle;
        public TextMeshProUGUI windowDescription;
        public Button confirmButton;
        public Button cancelButton;
        public Animator mwAnimator;

        // Content
        public Sprite icon;
        public string titleText = "Title";
        [TextArea] public string descriptionText = "Description here";

        // Events
        public UnityEvent onConfirm;
        public UnityEvent onCancel;

        // Settings
        public bool sharpAnimations = false;
        public bool useCustomValues = false;
        public bool destroyOnClose = false;

        public bool isOn = false;

        private AudioSource _audio;

        void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

        void Start()
        {
            if (mwAnimator == null)
                mwAnimator = gameObject.GetComponent<Animator>();

            if (confirmButton != null)
                confirmButton.onClick.AddListener(onConfirm.Invoke);

            if (cancelButton != null)
                cancelButton.onClick.AddListener(onCancel.Invoke);

            if (useCustomValues == false)
                UpdateUI();
        }

        public void UpdateUI()
        {
            try
            {
                windowIcon.sprite = icon;
                windowTitle.text = titleText;
                windowDescription.text = descriptionText;
            }

            catch
            {
                Debug.LogWarning("Modal Window - Cannot update the content due to missing variables.", this);
            }
        }

        public void OpenWindow()
        {
            if (isOn == false)
            {
                if (sharpAnimations == false)
                    mwAnimator.CrossFade("Fade-in", 0.1f);
                else
                    mwAnimator.Play("Fade-in");

                isOn = true;
            }

            _audio.Play();
        }

        public void OpenWindow(string title, string message, bool showConfigureButton, bool showRestartButton)
        {
            titleText = title;
            descriptionText = message;
            confirmButton.gameObject.SetActive(showRestartButton);
            cancelButton.gameObject.SetActive(showConfigureButton);
            UpdateUI();
            
            if (isOn == false)
            {
                if (sharpAnimations == false)
                    mwAnimator.CrossFade("Fade-in", 0.1f);
                else
                    mwAnimator.Play("Fade-in");

                isOn = true;
            }

            _audio.Play();
        }

        public void CloseWindow()
        {
            if (isOn == true)
            {
                if (sharpAnimations == false)
                    mwAnimator.CrossFade("Fade-out", 0.1f);
                else
                    mwAnimator.Play("Fade-out");

                isOn = false;

                if (destroyOnClose == true)
                    StartCoroutine("DestroyModal");
            }
        }

        public void AnimateWindow()
        {
            if (isOn == false)
            {
                if (sharpAnimations == false)
                    mwAnimator.CrossFade("Fade-in", 0.1f);
                else
                    mwAnimator.Play("Fade-in");

                isOn = true;
            }

            else
            {
                if (sharpAnimations == false)
                    mwAnimator.CrossFade("Fade-out", 0.1f);
                else
                    mwAnimator.Play("Fade-out");

                isOn = false;

                if (destroyOnClose == true)
                    StartCoroutine("DestroyModal");
            }
        }

        IEnumerator DestroyModal()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}