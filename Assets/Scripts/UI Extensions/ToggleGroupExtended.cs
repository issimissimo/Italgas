using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ToggleGroupExtended : ToggleGroup
{
     /// <summary>
        /// Notify the group that the given toggle is enabled.
        /// </summary>
        /// <param name="toggle">The toggle that got triggered on.</param>
        /// <param name="sendCallback">If other toggles should send onValueChanged.</param>
        public new void NotifyToggleOn(Toggle toggle, bool sendCallback = true)
        {
            ValidateToggleIsInGroup(toggle);
            // disable all toggles in the group
            for (var i = 0; i < m_Toggles.Count; i++)
            {
                if (m_Toggles[i] == toggle)
                    continue;

                if (sendCallback)
                    m_Toggles[i].isOn = false;
                else
                    m_Toggles[i].SetIsOnWithoutNotify(false);
            }

            print("AAAAAAAAAAAAAA");
        }

        private void ValidateToggleIsInGroup(Toggle toggle)
        {
            if (toggle == null || !m_Toggles.Contains(toggle))
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] {toggle, this}));
        }
}
