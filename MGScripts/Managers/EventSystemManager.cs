using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemManager : MonoBehaviour {

    private static EventSystemManager dontDestroy; // Ensures that there is only one instance of this gameObject

    #region Don't Destroy
    private void Awake()
    {
        DontDestroyBetweenScenes();
    }

    /// <summary>
    /// Ensures that this game object is not destroyed between scenes, and thus does not have to be re-loaded in
    /// </summary>
    private void DontDestroyBetweenScenes()
    {
        // Ensure that this is the only object we are not destroying, and that we create no duplicates
        if (dontDestroy == null)
        {
            DontDestroyOnLoad(this.gameObject);     // We don't want this object to be destroyed any time we change scenes
            dontDestroy = this;
        }
        else if (dontDestroy != this)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion
}
