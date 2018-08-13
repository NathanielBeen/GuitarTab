using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    //create a set of subcalsses that implement a createattributes method to create the IPropertyMenu for each effect type
    //create a set of subclasses to hold the initial variables, with overriden equals to check if the item has changed
    //create a wrapper class to hold a list of menu items for each individual effect. the main effectproperties bindinglist will 
    //link with an event or delegate to this class, which will add or remove items from the list as effects change
    public class EffectProperties : BasePropertyMenu
    {
        private IEffect effect;
        private EffectType init_type;
        private EffectPosition position;

        private NoteTreeNode first;
        private NoteTreeNode second;

        public List<string> Types { get; }

        private EffectType current_type;
        public string CurrentType
        {
            get { return current_type.getMenuName(); }
            set
            {
                SetProperty(ref current_type, EffectTypeExtensions.getEffectTypeFromString(value));
                changeSelectedType(current_type);
            }
        }

        private BaseEffectMenuStrategy current_strategy;
        public BaseEffectMenuStrategy CurrentStrategy
        {
            get { return current_strategy; }
            set { SetProperty(ref current_strategy, value); }
        }

        public EffectProperties(GuiCommandExecutor exec, NodeClick c, IEffect e, EffectPosition pos, NoteTreeNode f, NoteTreeNode s)
            :base(c, exec)
        { 
            effect = e;
            init_type = effect?.Type ?? EffectType.No_Type;
            position = pos;

            first = f;
            second = s;

            Types = genValidEffectTypes();
            CurrentStrategy = createNewStrategy(init_type);
            CurrentType = init_type.getMenuName();
        }

        public List<string> genValidEffectTypes()
        {
            if (first == null || second == null) { return EffectTypeExtensions.getSingleEffectNames(position); }
            else { return EffectTypeExtensions.getAllEffectNames(position); }
        }

        public BaseEffectMenuStrategy createNewStrategy(EffectType type)
        {
            switch (type)
            {
                case EffectType.Vibrato:
                    return new VibratoMenuStrategy(executor, ref_click, effect as Vibrato);
                case EffectType.Bend:
                    return new BendMenuStrategy(executor, ref_click, effect as Bend);
                case EffectType.Slide:
                    return new SlideMenuStrategy(executor, ref_click, first, second, effect as Slide);
                case EffectType.Tie:
                case EffectType.HOPO:
                    return new MultiEffectMenuStrategy(executor, ref_click, type, first, second);
                default:
                    return new BaseEffectMenuStrategy(executor, ref_click, type);
            }
        }

        public void changeSelectedType(EffectType new_type)
        {
            if (CurrentStrategy.getType() != new_type)
            {
                CurrentStrategy = createNewStrategy(new_type);
            }
        }

        public override void resetToDefault()
        {
            if (current_type != init_type) { CurrentType = init_type.getMenuName(); }
            else { CurrentStrategy.resetToDefault(); }
        }

        public override void submitChanges()
        {
            if (init_type != EffectType.No_Type && current_type == EffectType.No_Type) { executor.executeRemoveEffectFromNote(getClickCopy(), effect); }
            else if (current_type != EffectType.No_Type && (init_type != current_type || CurrentStrategy.effectChanged())) { CurrentStrategy.addEffect(); }
        }
    }

    public class BaseEffectMenuStrategy : BaseInputViewModel
    {
        protected EffectType type;
        protected GuiCommandExecutor executor;
        protected NodeClick click;

        public BaseEffectMenuStrategy(GuiCommandExecutor gui, NodeClick c, EffectType t)
        {
            type = t;
            click = c;
            executor = gui;
        }

        public virtual bool effectChanged() { return false; }

        public virtual void addEffect() { executor.executeAddEffectToNoteProp(getClickCopy(), type); }

        public virtual void resetToDefault() { }

        public EffectType getType() { return type; }

        public NodeClick getClickCopy()
        {
            NodeClick new_click = new NodeClick(click.Point);
            new_click.PartNode = click.PartNode;
            new_click.MeasureNodes.AddRange(click.MeasureNodes);
            new_click.ChordNodes.AddRange(click.ChordNodes);
            new_click.NoteNodes.AddRange(click.NoteNodes);
            new_click.EffectNode = click.EffectNode;
            new_click.Selected = click.Selected;

            return new_click;
        }
    }

    public class VibratoMenuStrategy : BaseEffectMenuStrategy
    {
        private readonly bool init_wide;

        private bool wide;
        public bool Wide
        {
            get { return wide; }
            set { SetProperty(ref wide, value); }
        }

        public VibratoMenuStrategy(GuiCommandExecutor gui, NodeClick click, Vibrato vib)
            : base(gui, click, EffectType.Vibrato)
        {
            init_wide = vib?.Wide ?? false;
            Wide = init_wide;
        }

        public override bool effectChanged() { return Wide != init_wide; }

        public override void addEffect() { executor.executeAddVibratoToNoteProp(getClickCopy(), wide); }

        public override void resetToDefault() { Wide = init_wide; }
    }

    public class BendMenuStrategy : BaseEffectMenuStrategy
    {
        public const double MIN_BEND = 0;
        public const double MAX_BEND = 2;

        private readonly double init_amount;
        private readonly bool init_returns;

        private string amount;
        public string Amount
        {
            get { return amount.ToString(); }
            set
            {
                string error = AmountError;
                setDoubleProperty(ref amount, value, MIN_BEND, MAX_BEND, ref error);
                AmountError = error;
            }
        }

        private string amount_error;
        public string AmountError
        {
            get { return amount_error; }
            set { SetProperty(ref amount_error, value); }
        }

        private bool returns;
        public bool Returns
        {
            get { return returns; }
            set { SetProperty(ref returns, value); }
        }

        public BendMenuStrategy(GuiCommandExecutor gui, NodeClick click, Bend bend)
            : base(gui, click, EffectType.Bend)
        {
            init_amount = bend?.Amount ?? 1;
            init_returns = bend?.BendReturns ?? false;
            Amount = init_amount.ToString();
            Returns = init_returns;
        }

        public override bool effectChanged()
        {
            if (AmountError == null || !Double.TryParse(amount, out double amount_d)) { return false; }
            return amount_d != init_amount || Returns != init_returns;
        }

        public override void addEffect()
        {
            double am = Double.TryParse(amount, out double amount_d) ? amount_d : 1;
            executor.executeAddBendToNoteProp(getClickCopy(), am, returns);
        }

        public override void resetToDefault()
        {
            Amount = init_amount.ToString();
            Returns = init_returns;
        }

        public ICollection<string> validateAmount(string new_amount)
        {
            var errors = new List<string>();
            double dbl_amount = -1;

            if (double.TryParse(new_amount, out dbl_amount))
            {
                if (dbl_amount <= 0 || dbl_amount > 2) { errors.Add("must be between 0 and 2"); }
            }
            else { errors.Add("Must be a number"); }
            return errors;
        }
    }

    public class MultiEffectMenuStrategy : BaseEffectMenuStrategy
    {
        protected NoteTreeNode first_note;
        protected NoteTreeNode second_note;

        public MultiEffectMenuStrategy(GuiCommandExecutor gui, NodeClick click, EffectType type, NoteTreeNode first, NoteTreeNode second)
            :base(gui, click, type)
        {
            first_note = first;
            second_note = second;
        }

        public override void addEffect()
        {
            var new_click = getClickCopy();
            new_click.NoteNodes.Clear();
            new_click.NoteNodes.Add(first_note);
            new_click.NoteNodes.Add(second_note);
            executor.executeAddMultiEffectToNoteProp(new_click, type);
        }
    }

    public class SlideMenuStrategy : MultiEffectMenuStrategy
    {
        private bool init_legato;

        private bool legato;
        public bool Legato
        {
            get { return legato; }
            set { SetProperty(ref legato, value); }
        }

        public SlideMenuStrategy(GuiCommandExecutor gui, NodeClick click, NoteTreeNode first, NoteTreeNode second, Slide init_slide)
            :base(gui, click, EffectType.Slide, first, second)
        {
            init_legato = init_slide?.Legato ?? false;
            legato = init_legato;
        }

        public override bool effectChanged() { return legato != init_legato; }

        public override void addEffect()
        {
            var new_click = getClickCopy();
            new_click.NoteNodes.Clear();
            new_click.NoteNodes.Add(first_note);
            new_click.NoteNodes.Add(second_note);
            executor.executeAddSlideToNoteProp(new_click, legato);
        }

        public override void resetToDefault() { Legato = init_legato; }
    }
}
