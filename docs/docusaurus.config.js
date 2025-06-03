// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const lightCodeTheme = require("prism-react-renderer/themes/github");
const darkCodeTheme = require("prism-react-renderer/themes/dracula");

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: "Cocos2d-Mono",
  tagline: "GO COCOS NUTS!",
  url: "https://cocos2d-mono.dev",
  baseUrl: "/",
  onBrokenLinks: "throw",
  onBrokenMarkdownLinks: "warn",
  favicon: "img/favicon.ico",

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: "brandmooffin", // Usually your GitHub org/user name.
  projectName: "cocos2d-mono", // Usually your repo name.

  // Even if you don't use internalization, you can use this field to set useful
  // metadata like html lang. For example, if your site is Chinese, you may want
  // to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: "en",
    locales: ["en"],
  },

  plugins: [require.resolve("@cmfcmf/docusaurus-search-local")],

  presets: [
    [
      "classic",
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          sidebarPath: require.resolve("./sidebars.js"),
        },
        blog: {
          showReadingTime: true,
        },
        theme: {
          customCss: require.resolve("./src/css/custom.css"),
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      navbar: {
        title: "Cocos2d-Mono",
        logo: {
          alt: "Cocos2d-Mono Logo",
          src: "img/logo.svg",
        },
        items: [
          {
            type: "doc",
            docId: "getting-started/introduction",
            position: "left",
            label: "Guides",
          },
          { to: "/blog", label: "Blog", position: "left" },
          {
            type: "doc",
            docId: "tutorials/introduction",
            position: "left",
            label: "Tutorials",
          },
          {
            href: "https://github.com/brandmooffin/cocos2d-mono",
            label: "GitHub",
            position: "right",
          },
        ],
      },
      footer: {
        style: "dark",
        links: [
          {
            title: "Docs",
            items: [
              {
                label: "Blog",
                to: "/blog",
              },
              {
                label: "Getting Started",
                to: "/docs/getting-started/introduction",
              },
              {
                label: "Tutorials",
                to: "/docs/tutorials/introduction",
              },
            ],
          },
          {
            title: "Community",
            items: [
              {
                label: "Issues",
                href: "https://github.com/brandmooffin/cocos2d-mono/issues",
              },
              {
                label: "Discussions",
                href: "https://github.com/brandmooffin/cocos2d-mono/discussions/landing",
              },
            ],
          },
          {
            title: "More",
            items: [
              {
                label: "GitHub",
                href: "https://github.com/brandmooffin/cocos2d-mono",
              },
            ],
          },
        ],
        copyright: `Copyright © ${new Date().getFullYear()} Cocos2d-Mono. Built with Docusaurus.`,
      },
      prism: {
        theme: lightCodeTheme,
        darkTheme: darkCodeTheme,
      },
    }),
};

module.exports = config;
