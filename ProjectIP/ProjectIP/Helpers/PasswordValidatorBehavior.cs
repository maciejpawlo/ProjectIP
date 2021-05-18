using Prism.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace ProjectIP.Helpers
{
    class PasswordValidatorBehavior : BehaviorBase<Entry>
    {
        public static readonly BindableProperty IsValidProperty =
           BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(EmailValidationBehavior), false, BindingMode.OneWayToSource);

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set
            {
                SetValue(IsValidProperty, value);
                System.Diagnostics.Debug.WriteLine($"Is True being set to: {value} by the PasswordValidatorBehavior");
            }
        }
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject.TextChanged += OnTextChanged; //wire method to event
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");
            IsValid = hasNumber.IsMatch(e.NewTextValue) && hasUpperChar.IsMatch(e.NewTextValue) && hasMinimum8Chars.IsMatch(e.NewTextValue);
            entry.TextColor = IsValid ? Color.Default : Color.Red;
        }
    }
}
