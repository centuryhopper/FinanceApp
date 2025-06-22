// stores/theme.ts
import { defineStore } from "pinia";

export const useThemeStore = defineStore("theme", {
  state: () => ({
    isDark: true, // default to dark
  }),
  actions: {
    toggleTheme() {
      this.isDark = !this.isDark;
      document.documentElement.setAttribute(
        "data-theme",
        this.isDark ? "dark" : "light"
      );
    },
  },
});
