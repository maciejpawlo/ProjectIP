using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Prism.Behaviors;
using Xamarin.Forms;

namespace ProjectIP.Helpers
{
    class EmailValidationBehavior : BehaviorBase<Entry>
    {
        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(EmailValidationBehavior), false, BindingMode.OneWayToSource);

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set
            {
                SetValue(IsValidProperty, value);
                System.Diagnostics.Debug.WriteLine($"Is True being set to: {value} by the EmailValidationBehavior");
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
            const string pattern =
                     @"^([0-9a-zA-Z]" + //Start with a digit or alphabetical
                     @"([\+\-_\.][0-9a-zA-Z]+)*" + // No continuous or ending +-_. chars in email
                     @")+" +
                     @"@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";
            var regex = new Regex(pattern);
            IsValid = regex.IsMatch(e.NewTextValue);
            entry.TextColor = IsValid ? Color.Default : Color.Red;
        }
    }
}
