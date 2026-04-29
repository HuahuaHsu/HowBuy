<template>
  <div v-if="tags && tags.length > 0" class="item-promo-tags">
    <el-tag
      v-for="(tag, index) in tags"
      :key="index"
      size="small"
      type="danger"
      effect="plain"
      class="promo-mini-tag"
    >
      {{ tag }}
    </el-tag>
  </div>
</template>

<script setup lang="ts">
interface Props {
  tags?: string[];
}

withDefaults(defineProps<Props>(), {
  tags: () => []
});
</script>

<style scoped>
.item-promo-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  margin-bottom: 4px;
}
.promo-mini-tag {
  font-size: 10px;
  padding: 0 4px;
  height: 18px;
  line-height: 16px;
}
</style>
