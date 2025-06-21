import 'bootstrap'; // imports JavaScript behavior for dropdowns, navbar toggles, etc.
import 'bootstrap/dist/css/bootstrap.min.css';
import { createPinia } from "pinia";
import 'sweetalert2/dist/sweetalert2.min.css';
import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import "./style.css";


const app = createApp(App);
app.use(createPinia());
app.use(router);

app.mount("#app");
