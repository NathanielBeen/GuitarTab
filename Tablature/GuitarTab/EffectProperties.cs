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
    public class EffectProperties : BaseValidator, IPropertyMenu
    {
        private IEffect effect;
        private EffectType init_type;

        private GuiCommandExecutor executor;
        private NodeClick click;
        private Note first;
        private Note second;

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

        public EffectProperties(GuiCommandExecutor exec, NodeClick c, IEffect e, Note f, Note s)
        { 
            effect = e;
            init_type = effect?.Type ?? EffectType.No_Type;

            executor = exec;
            click = c;
            first = f;
            second = s;

            Types = genValidEffectTypes();
            CurrentStrategy = createNewStrategy(init_type);
            CurrentType = init_type.getMenuName();
        }

        public List<string> genValidEffectTypes()
        {
            if (first == null || second == null) { return EffectTypeExtensions.getSingleEffectNames(); }
            else { return EffectTypeExtensions.getAllEffectNames(); }
        }

        public BaseEffectMenuStrategy createNewStrategy(EffectType type)
        {
            switch (type)
            {
                case EffectType.Vibrato:
                    return new VibratoMenuStrategy(executor, click, effect as Vibrato);
                case EffectType.Bend:
                    return new BendMenuStrategy(executor, click, effect as Bend);
                case EffectType.Slide:
                    return new SlideMenuStrategy(executor, click, first, second, effect as Slide);
                case EffectType.Tie:
                case EffectType.HOPO:
                    return new MultiEffectMenuStrategy(executor, click, type, first, second);
                default:
                    return new BaseEffectMenuStrategy(executor, click, type);
            }
        }

        public void changeSelectedType(EffectType new_type)
        {
            if (CurrentStrategy.getType() != new_type)
            {
                CurrentStrategy = createNewStrategy(new_type);
            }
        }

        public void resetToDefault()
        {
            if (current_type != init_type) { CurrentType = init_type.getMenuName(); }
            else { CurrentStrategy.resetToDefault(); }
        }

        public void submitChanges()
        {
            if (init_type != EffectType.No_Type && current_type == EffectType.No_Type) { executor.executeRemoveEffectFromNote(click, effect); }
            else if (current_type != EffectType.No_Type && (init_type != current_type || CurrentStrategy.effectChanged())) { CurrentStrategy.addEffect(); }
        }
    }

    public class BaseEffectMenuStrategy : BaseValidator
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

        public virtual void addEffect() { executor.executeAddEffectToNoteProp(click, type); }

        public virtual void resetToDefault() { }

        public EffectType getType() { return type; }
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

        public override void addEffect() { executor.executeAddVibratoToNoteProp(click, wide); }

        public override void resetToDefault() { Wide = init_wide; }
    }

    public class BendMenuStrategy : BaseEffectMenuStrategy
    {
        private readonly double init_amount;
        private readonly bool init_returns;

        private double amount;
        public string Amount
        {
            get { return amount.ToString(); }
            set
            {
                if (ValidateProperty(nameof(Amount), value, validateAmount))
                {
                    SetProperty(ref amount, Double.Parse(value));
                }
            }
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

        public override bool effectChanged() { return amount != init_amount || Returns != init_returns; }

        public override void addEffect() { executor.executeAddBendToNoteProp(click, amount, returns); }

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
        protected Note first_note;
        protected Note second_note;

        public MultiEffectMenuStrategy(GuiCommandExecutor gui, NodeClick click, EffectType type, Note first, Note second)
            :base(gui, click, type)
        {
            first_note = first;
            second_note = second;
        }

        public override void addEffect() { executor.executeAddMultiEffectToNoteProp(click, first_note, second_note, type); }
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

        public SlideMenuStrategy(GuiCommandExecutor gui, NodeClick click, Note first, Note second, Slide init_slide)
            :base(gui, click, EffectType.Slide, first, second)
        {
            init_legato = init_slide?.Legato ?? false;
            legato = init_legato;
        }

        public override bool effectChanged() { return legato != init_legato; }

        public override void addEffect() { executor.executeAddSlideToNoteProp(click, first_note, second_note, legato); }

        public override void resetToDefault() { Legato = init_legato; }
    }
}
