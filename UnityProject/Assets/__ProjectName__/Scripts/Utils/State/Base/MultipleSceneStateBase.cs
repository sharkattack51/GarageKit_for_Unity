using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GarageKit
{
    public class MultipleSceneStateBase : StateBase, IMultipleSceneState
    {		
        [Header("MutipleSceneStateBase")]
        public string loadSceneName;
        public bool asAsyncLoad = false;
        public bool asAdditiveLoad = false;

        private SceneRepositoryBase sceneRepository;
        public SceneRepositoryBase SceneRepository { get{ return sceneRepository; } }


        public override void StateStart(object context)
        {
            base.StateStart(context);

            base.updateEnable = false;

            if(loadSceneName != "" && SceneManager.GetActiveScene().name != loadSceneName)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.sceneUnloaded += OnSceneUnloaded;

                LoadSceneMode mode = asAdditiveLoad ? LoadSceneMode.Additive : LoadSceneMode.Single;
                if(asAsyncLoad)
                    SceneManager.LoadSceneAsync(loadSceneName, mode);
                else
                    SceneManager.LoadScene(loadSceneName, mode);
            }
            else
            {
                sceneRepository = FindObjectOfType<SceneRepositoryBase>();
                this.SceneLoaded();
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            sceneRepository = FindObjectOfType<SceneRepositoryBase>();

            this.SceneLoaded();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            this.SceneUnloaded();
        }

#region override functions
        public virtual void SceneLoaded()
        {
            base.updateEnable = true;
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

        public override void StateExit()
        {
            base.StateExit();
        }

        public virtual void SceneUnloaded()
        {

        }
#endregion
    }
}
