using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class IAP : MonoBehaviour, IStoreListener {

	void Start() {
		var module = StandardPurchasingModule.Instance();
		ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
		builder.AddProduct("com.medbe.trumpet_saga.adblock", ProductType.NonConsumable);
	}


	public void OnInitialized(IStoreController isc, IExtensionProvider iep) {

	}

	public void OnInitializeFailed(InitializationFailureReason ifr) {

	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs pev) {
		PurchaseProcessingResult r = PurchaseProcessingResult.Pending;
		return r;
	}

	public void OnPurchaseFailed(Product p, PurchaseFailureReason pfr) {

	}
}