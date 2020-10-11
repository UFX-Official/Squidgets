using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    // Change from int to LobbyListing custom class
    private List<LobbyListing> lobbyListings = new List<LobbyListing>();
    public Transform lobbyListParent;
    public GameObject noLobbyText;
    
    private void Start()
    {
        // Create Test Lobby's
        lobbyListings.Add(new LobbyListing("Ultimate Squish Masters", GameMode.FreeRoam, Map.Default, 6));
        lobbyListings.Add(new LobbyListing("Midget Meisters 2020", GameMode.FreeRoam, Map.Default, 10));
        lobbyListings.Add(new LobbyListing("Tinkletot's Tater Tasting", GameMode.FreeRoam, Map.Default, 3));
        
        // Display Lobby Listings
        UpdateLobbyListings();
    }

    private void UpdateLobbyListings()
    {
        // If there's no active listings, display a notice and back out.
        if (lobbyListings.Count == 0)
        {
            if (!noLobbyText.activeInHierarchy)
                noLobbyText.SetActive(true);

            return;
        }

        // If an active listing is found, hide the no listing notice
        if (noLobbyText.activeInHierarchy)
            noLobbyText.SetActive(false);

        // Loop through the listings and generate interactable buttons for each one.
        for (int i = 0; i < lobbyListings.Count; i++)
        {
            GameObject listing = Instantiate(Resources.Load("UI/LobbyListing"), lobbyListParent) as GameObject;
            LobbyListingManager lm = listing.GetComponent<LobbyListingManager>();
            lm.Initialize(lobbyListings[i]);
        }
        
    }
}
