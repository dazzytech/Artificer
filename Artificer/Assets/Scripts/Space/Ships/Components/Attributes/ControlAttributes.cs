using Space.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Ship.Components.Attributes
{
    public class ControlAttributes : ComponentAttributes
    {
        public List<FSM> Agent;

        public ICustomScript Script;
    }
}
