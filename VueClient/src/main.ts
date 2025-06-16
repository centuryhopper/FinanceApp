import { createPinia } from "pinia";
import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import "./style.css";
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap' // imports JavaScript behavior for dropdowns, navbar toggles, etc.


const app = createApp(App);
app.use(createPinia());
app.use(router);

app.mount("#app");
