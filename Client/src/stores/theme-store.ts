import { ref, watchEffect, onMounted } from "vue";

const isDark = ref(true);

export function useTheme() {
  const toggleTheme = () => {
    isDark.value = !isDark.value;
    setThemeAttribute();
  };

  const setThemeAttribute = () => {
    const theme = isDark.value ? "dark" : "light";
    // document.documentElement.setAttribute("data-theme", theme); // for custom CSS
    document.documentElement.setAttribute("data-bs-theme", theme); // for Bootstrap 5.3+
  };

  onMounted(() => {
    const stored = localStorage.getItem("theme");
    if (stored === "light") isDark.value = false;
    setThemeAttribute();
  });

  watchEffect(() => {
    const theme = isDark.value ? "dark" : "light";
    localStorage.setItem("theme", theme);
    setThemeAttribute();
  });

  return { isDark, toggleTheme };
}
