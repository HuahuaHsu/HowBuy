import { createApp } from 'vue';
import { createPinia } from 'pinia';
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate';
import ElementPlus from 'element-plus';
import 'element-plus/dist/index.css';
import * as ElementPlusIconsVue from '@element-plus/icons-vue';

import App from './App.vue';
import router from './router';

import './assets/main.css';

const app = createApp(App);
const pinia = createPinia();

pinia.use(piniaPluginPersistedstate);

//啟用 Element Plus 並設定中文
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import zhTw from 'element-plus/es/locale/lang/zh-tw'  // 繁體中文
import './styles/theme.css' // 覆寫主色調為橘紅色 #EE4D2D

const app = createApp(App)

// 註冊所有圖示
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component);
}

//註冊元件
app.use(ElementPlus, {
    locale: zhTw,
})

app.mount('#app')
