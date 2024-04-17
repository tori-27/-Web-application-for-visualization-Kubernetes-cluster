<template>
  <aside :class="`${is_expanded && 'is-expanded'}`">

    <div class="logo">
      <img src="../assets/logo.svg" alt="Vue">
      <h3>KubeCanvas</h3>
    </div>

    <div class="menu-toggle-wrap">
      <button class="menu-toggle" @click="ToggleMenu">
        <span class="material-icons">keyboard_double_arrow_right</span>
      </button>
    </div>

    <div class="menu">
      <router-link class="button" to="/">
        <span class="material-icons">dashboard</span>
        <span class="text">Dashboard</span>
      </router-link>
      <router-link class="button" to="/about">
        <span class="material-icons">data_usage</span>
        <span class="text">Overview</span>
      </router-link>
      <router-link class="button" to="/settings">
        <span class="material-icons">settings</span>
        <span class="text">Settings</span>
      </router-link>
    </div>
    <div class="flex"></div>

  </aside>
</template>

<script setup>
import {ref} from 'vue'

const is_expanded = ref(localStorage.getItem("is_expanded") === "true")
const ToggleMenu = () => {
  is_expanded.value = !is_expanded.value

  localStorage.setItem("is_expanded", is_expanded.value)
}


</script>

<style Lang="scss" scoped>
aside {
  display: flex;
  flex-direction: column;
  width: calc(2rem + 44px);
  min-height: 100vh;
  overflow: hidden;
  padding: 1rem;

  background-color: var(--dark);
  color: var(--light);

  transition: 0.2s ease-out;

  .flex {
    flex: 1 1 0;
  }

  .logo {
    margin-bottom: 1.5rem;
    align-items: center;
    gap: 1rem;
    display: flex;

    img {
        width: 2.5rem;
    }
  }

  .menu-toggle-wrap{
    display: flex;
    justify-content: flex-end;
    margin-bottom: 1rem;

    position: relative;
    top: 0;
    transition: 0.2s ease-out;

    .menu-toggle {
      transition: 0.2s ease-out;

      .material-icons {
        font-size: 2rem;
        color: var(--light);
        transition: 0.2s ease-out;
      }
      &:hover {
        .material-icons {
          color: var(--primary);
          transform: translateX(0.5rem);
        }
      }
    }
  }
  h3, .button .text {
    opacity: 0;
    transition: 0.3s ease-out;
  }

  h3 {
    color: var(--primary);
    font-size: 0.875rem;
    margin-bottom: 0.5rem;
    text-transform: uppercase;
    padding-top: 0.5rem;
  }
  .menu {
    margin: 0 -1rem;

    .button {
      display: flex;
      align-items: center;
      text-decoration: none;

      padding: 2rem 1rem;
      transition: 0.2s ease-out;

      .material-icons {
        font-size: 2rem;
        color: var(--light);
        margin-right: 1rem;
        transition: 0.2s ease-out;
      }

      .text {
        color: var(--light);
        transition: 0.2s ease-out;
      }

      &:hover, &.router-link-exact-active{
        background-color: var(--dark-alt);

        .material-icons, .text {
          color: var(--primary);
        }
      }
      &.router-link-exact-active {
        border-right: 5px solid var(--primary);
      }
    }
  }

  &.is-expanded {
    width: var(--sidebar-width);

    .menu-toggle-wrap{
      top: -3rem;
      .menu-toggle {
        transform: rotate(-180deg);
      }
    }
    h3, .button .text {
      opacity: 1;
    }

    .button {
      .material-icons{
        margin-right: 0.5rem;
      }
    }
  }

  @media (max-width: 768px){
    position: fixed;
    z-index: 99;
  }
}
</style>