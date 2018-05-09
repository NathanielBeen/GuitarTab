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
}
