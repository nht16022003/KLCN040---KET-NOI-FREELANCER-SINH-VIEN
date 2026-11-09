(() => {
  document.querySelectorAll('[data-password-toggle]').forEach((button) => {
    const input = document.getElementById(button.dataset.passwordToggle);
    if (!input) return;

    button.addEventListener('click', () => {
      const showPassword = input.type === 'password';
      input.type = showPassword ? 'text' : 'password';
      const icon = button.querySelector('i');
      if (icon) {
        icon.classList.toggle('bi-eye', !showPassword);
        icon.classList.toggle('bi-eye-slash', showPassword);
      }
      button.setAttribute('aria-label', `${showPassword ? 'Ẩn' : 'Hiện'} mật khẩu`);
      button.setAttribute('aria-pressed', String(showPassword));
    });
  });
})();
