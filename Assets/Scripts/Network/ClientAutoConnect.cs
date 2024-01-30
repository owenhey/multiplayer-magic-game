using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FishNet.Example;
using FishNet.Managing;
using UnityEngine;
using UnityEngine.UI;

namespace Net{
    public class ClientAutoConnect : MonoBehaviour {
        [SerializeField] private GameObject _networkButtons;
        [SerializeField] private Image _fadeInImage;
        private void Awake() {
            if (Application.isEditor) {
                _networkButtons.SetActive(true);
                _fadeInImage.gameObject.SetActive(false);
            }
            else {
                _fadeInImage.DOFade(0, .5f).SetDelay(1.5f).From(1).OnComplete(() => {
                    _fadeInImage.gameObject.SetActive(false);
                });
                var _networkManager = FindObjectOfType<NetworkManager>();
                if (NetworkSettings.connectAsClient) {
                    _networkManager.ClientManager.StartConnection();
                }
                else {
                    _networkManager.ServerManager.StartConnection();
                }
            }
            
        }
    }
}