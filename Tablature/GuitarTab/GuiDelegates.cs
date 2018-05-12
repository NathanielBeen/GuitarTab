using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public interface IDelegate 
    {
        void subscribeAction(Action action, TreeNode node);
        void unsubscribeAction(Action action);
        void invokeDelegate();
    }

    public interface IMouseDelegate
    {
        void subscribeAction(Action<MouseClick> action, TreeNode node);
        void unsubscribeAction(Action<MouseClick> action);
        void invokeDelegate(MouseClick click);
    }

    public class UnorderedDelegate : IDelegate
    {
        private List<Action> actions;

        public UnorderedDelegate()
        {
            actions = new List<Action>();
        }

        public void subscribeAction(Action action, TreeNode node)
        {
            actions.Add(action);
        }

        public void unsubscribeAction(Action action)
        {
            actions.Remove(action);
        }

        public void invokeDelegate()
        {
            foreach (Action action in actions) { action.Invoke(); }
        }
    }

    public class OrderedDelegate : IDelegate
    {
        private Dictionary<Action, Position> actions;

        public OrderedDelegate()
        {
            actions = new Dictionary<Action, Position>();
        }

        public void subscribeAction(Action action, TreeNode node)
        {
            IPosition model = node.BaseObject as IPosition;
            if (model != null) { actions.Add(action, model.Position); }
        }

        public void unsubscribeAction(Action action)
        {
            actions.Remove(action);
        }

        public void invokeDelegate()
        {
            var ordered_actions = (from action in actions.Keys
                                   orderby actions[action].Index
                                   select action);
            foreach (Action action in ordered_actions) { action.Invoke(); }
        }
    }

    public class UnorderedMouseDelegate : IMouseDelegate
    {
        List<Action<MouseClick>> actions;

        public UnorderedMouseDelegate()
        {
            actions = new List<Action<MouseClick>>();
        }

        public void subscribeAction(Action<MouseClick> action, TreeNode node)
        {
            actions.Add(action);
        }

        public void unsubscribeAction(Action<MouseClick> action)
        {
            actions.Remove(action);
        }

        public void invokeDelegate(MouseClick click)
        {
            foreach (Action<MouseClick> action in actions) { action.Invoke(click); }
        }
    }

    public class OrderedMouseDelegate : IMouseDelegate
    {
        private Dictionary<Action<MouseClick>, Position> actions;

        public OrderedMouseDelegate()
        {
            actions = new Dictionary<Action<MouseClick>, Position>();
        }

        public void subscribeAction(Action<MouseClick> action, TreeNode node)
        {
            IPosition model = node.BaseObject as IPosition;
            if (model != null) { actions.Add(action, model.Position); }
        }

        public void unsubscribeAction(Action<MouseClick> action)
        {
            actions.Remove(action);
        }

        public void invokeDelegate(MouseClick click)
        {
            var ordered_actions = (from action in actions.Keys
                                   orderby actions[action].Index
                                   select action);
            foreach (Action<MouseClick> action in ordered_actions) { action.Invoke(click); }
        }
    }
}
