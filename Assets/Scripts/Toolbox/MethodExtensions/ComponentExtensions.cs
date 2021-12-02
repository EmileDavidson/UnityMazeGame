using System.Collections.Generic;
using UnityEngine;

namespace Toolbox.MethodExtensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// adds the component to the given component's game object. 
        /// </summary>
        /// <param name="component">Component.</param>
        /// <returns>attached component</returns>
        public static T AddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.AddComponent<T>();
        }
        
        /// <summary>
        /// Gets a component attached to the given component's GameObject.
        /// If one isn't found, a new one is attached and returned.
        /// </summary>
        /// <param name="component">component.</param>
        /// <returns>the component we wanted to add or get</returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (component.HasComponent<T>()) { return component.GetComponent<T>(); }
            else {return component.AddComponent<T>();}
        }

        /// <summary>
        /// checks whether a component's gameobject has the given component or not.
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }

        /// <summary>
        /// checks if object has component and returns that and out's the component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="comp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>returns if there was a component</returns>
        public static bool HasAndGetComponent<T>(this Component component, out Component comp) where T : Component
        {
            comp = component.GetComponent<T>() ? component.GetComponent<T>() : null;
            return comp != null;
        }

        /// <summary>
        /// checks if object parent has component and returns that and out's the component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="comp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>returns if there was a component</returns>
        public static bool HasAndGetComponentInParent<T>(this Component component, out Component comp) where T : Component
        {
            comp = component.GetComponentInParent<T>() ? component.GetComponentInParent<T>() : null;
            return comp != null;
        }

        /// <summary>
        /// gets or add script from / to add children of GameObject and returns the list of components
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Component> GetOrAddComponentAllChildren<T>(this Component component) where T : Component
        {
            List<Component> components = new List<Component>();
            List<GameObject> childGameObjects = component.gameObject.GetAllChildrenGameObjects();

            foreach (GameObject child in childGameObjects)
            {
                if (child.TryGetComponent<T>(out var comp))
                {
                    components.Add(comp);
                    continue;
                }
                var addedComp = child.AddComponent<T>();
                components.Add(addedComp);
            }

            return components;
        }
    }
}
