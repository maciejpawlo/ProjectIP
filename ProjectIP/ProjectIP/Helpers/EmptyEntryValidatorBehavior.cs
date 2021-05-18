using Prism.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ProjectIP.Helpers
{
    class EmptyEntryValidatorBehavior : BehaviorBase<Entry>
    {
        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(EmailValidationBehavior), false, BindingMode.OneWayToSource);

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set
            {
                SetValue(IsValidProperty, value);
                System.Diagnostics.Debug.WriteLine($"Is True being set to: {value} by the EmptyEntryValidatorBehavior");
            }
        }
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject.TextChanged += OnTextChanged; //wire method to event
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e) //validation here
        {
            var entry = sender as Entry;
            IsValid = !string.IsNullOrEmpty(e.NewTextValue);
            entry.TextColor = IsValid ? Color.Default : Color.Red;
        }
    }
}
