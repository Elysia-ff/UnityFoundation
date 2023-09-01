using Elysia.Audios;
using Elysia.Components;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elysia.UI
{
    public class AudioHUD : HUDBase<AudioHUD>
    {
        [SerializeField] private HyperLinkComponent _licenseHyperLink;

        [SerializeField] private Button _startAsBGMBtn;
        [SerializeField] private Button _startAsSFXBtn;
        [SerializeField] private Button _stopBtn;

        [SerializeField] private Slider _masterVolume;
        [SerializeField] private Slider _bgmVolume;
        [SerializeField] private Slider _sfxVolume;

        private AudioSource _audioSource;

        protected override void Initialize()
        {
            base.Initialize();

            _licenseHyperLink.OnLinkClicked += OnLinkClicked;

            _startAsBGMBtn.onClick.AddListener(OnStartAsBGM);
            _startAsSFXBtn.onClick.AddListener(OnStartAsSFX);
            _stopBtn.onClick.AddListener(OnStop);

            _masterVolume.value = Game.Audio.MasterVolume;
            _masterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);

            _bgmVolume.value = Game.Audio.BGMVolume;
            _bgmVolume.onValueChanged.AddListener(OnBGMVolumeChanged);

            _sfxVolume.value = Game.Audio.SFXVolume;
            _sfxVolume.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        private void OnLinkClicked(int index, in TMP_LinkInfo linkInfo)
        {
            Application.OpenURL(linkInfo.GetLinkText());
        }

        private void OnStartAsBGM()
        {
            if (_audioSource != null)
            {
                Destroy(_audioSource);
            }

            Game.Audio.CreateAudioSource("Audios/Clips/Cloudy by Ikson.mp3", gameObject, EMixerType.BGM, true, 1f, (audioSource) =>
            {
                _audioSource = audioSource;
                _audioSource.Play();
            });
        }

        private void OnStartAsSFX()
        {
            if (_audioSource != null)
            {
                Destroy(_audioSource);
            }

            Game.Audio.CreateAudioSource("Audios/Clips/Cloudy by Ikson.mp3", gameObject, EMixerType.SFX, false, 1f, (audioSource) =>
            {
                _audioSource = audioSource;
                _audioSource.Play();
            });
        }

        private void OnStop()
        {
            if (_audioSource == null)
            {
                return;
            }

            _audioSource.Stop();
        }

        private void OnMasterVolumeChanged(float v)
        {
            Game.Audio.MasterVolume = v;
        }

        private void OnBGMVolumeChanged(float v)
        {
            Game.Audio.BGMVolume = v;
        }

        private void OnSFXVolumeChanged(float v)
        {
            Game.Audio.SFXVolume = v;
        }
    }
}
