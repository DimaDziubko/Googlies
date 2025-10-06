using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace _Game.Gameplay._Units.FSM
{
    public class WarriorFsm
    {
        [ShowInInspector, ReadOnly]
        private StateNode _current;
        private readonly Dictionary<Type, StateNode> _nodes = new();
        private readonly HashSet<ITransition> _anyTransitions = new();

        public void GameUpdate(float deltaTime)
        {
            var transition = GetTransition();
            if (transition != null) 
                ChangeState(transition.To);
            
            _current.State?.GameUpdate(deltaTime);
        }

        public void FixedUpdate()
        {
            _current.State?.FixedUpdate();
        }

        public void SetState(IWarriorState state)
        {
            _current = _nodes[state.GetType()];
            _current.State?.Enter();
        }
        
        public void SetStateByType(Type stateType)
        {
            if (_nodes.TryGetValue(stateType, out var node))
            {
                ChangeState(node.State);
            }
        }

        void ChangeState(IWarriorState state)
        {
            if (state == _current.State)
                return;
            
            var previousState = _current.State;
            var nextState = _nodes[state.GetType()].State;
            
            previousState?.Exit();
            nextState?.Enter();
            
            _current = _nodes[state.GetType()];
        }

        private ITransition GetTransition()
        {
            foreach (var transition in _anyTransitions)
                if (transition.Condition.Evaluate())
                    return transition;
            
            foreach (var transition in _current.Transitions)
                if (transition.Condition.Evaluate())
                    return transition;
            
            return null;
        }

        public void AddTransition(IWarriorState from, IWarriorState to, IPredicate condition) => 
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);

        public void AddAnyTransition(IWarriorState to, IPredicate condition) => 
            _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        
        private StateNode GetOrAddNode(IWarriorState state)
        {
            var node = _nodes.GetValueOrDefault(state.GetType());

            if (node == null)
            {
                node = new StateNode(state);
                _nodes.Add(state.GetType(), node);
            }
            
            return node;
        }

        class StateNode
        {
            [ShowInInspector, ReadOnly]
            public string Name => State.Name;
            public IWarriorState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IWarriorState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IWarriorState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }

        public void Cleanup()
        {
            foreach (var node in _nodes.Values)
            {
                node.State?.Cleanup();
            }
        }
    }
}