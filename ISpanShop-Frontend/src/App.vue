<script setup lang="ts">
import { ref } from 'vue'

const apiMessage = ref('目前尚未連線...')

const testConnection = async () => {
  try {
    // 🚀 關鍵：我們刻意輸入 C# 的絕對網址 (7125)，直接挑戰 CORS 警衛！
    const response = await fetch('https://localhost:7125/api/TestApi')

    // 如果警衛放行，我們就能順利拿到資料並解析成 JSON
    const data = await response.json()

    // 把拿到的訊息顯示在畫面上
    apiMessage.value = data.message

  } catch (error) {
    apiMessage.value = '連線失敗！被 CORS 擋住了，請看 F12 開發者工具。'
    console.error(error)
  }
}
</script>

<template>
  <div style="padding: 50px; text-align: center;">
    <h2>ISpanShop 前後端連線測試</h2>
    <button @click="testConnection" style="padding: 10px 20px; font-size: 16px;">
      點擊發送 API 請求
    </button>
    <p style="margin-top: 20px; font-size: 20px; color: blue;">
      {{ apiMessage }}
    </p>
  </div>
</template>
