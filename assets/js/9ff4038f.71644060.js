"use strict";(self.webpackChunkdocs=self.webpackChunkdocs||[]).push([[2353],{3905:(e,t,o)=>{o.d(t,{Zo:()=>c,kt:()=>d});var n=o(7294);function r(e,t,o){return t in e?Object.defineProperty(e,t,{value:o,enumerable:!0,configurable:!0,writable:!0}):e[t]=o,e}function a(e,t){var o=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),o.push.apply(o,n)}return o}function i(e){for(var t=1;t<arguments.length;t++){var o=null!=arguments[t]?arguments[t]:{};t%2?a(Object(o),!0).forEach((function(t){r(e,t,o[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(o)):a(Object(o)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(o,t))}))}return e}function s(e,t){if(null==e)return{};var o,n,r=function(e,t){if(null==e)return{};var o,n,r={},a=Object.keys(e);for(n=0;n<a.length;n++)o=a[n],t.indexOf(o)>=0||(r[o]=e[o]);return r}(e,t);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);for(n=0;n<a.length;n++)o=a[n],t.indexOf(o)>=0||Object.prototype.propertyIsEnumerable.call(e,o)&&(r[o]=e[o])}return r}var l=n.createContext({}),p=function(e){var t=n.useContext(l),o=t;return e&&(o="function"==typeof e?e(t):i(i({},t),e)),o},c=function(e){var t=p(e.components);return n.createElement(l.Provider,{value:t},e.children)},u={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},m=n.forwardRef((function(e,t){var o=e.components,r=e.mdxType,a=e.originalType,l=e.parentName,c=s(e,["components","mdxType","originalType","parentName"]),m=p(o),d=r,f=m["".concat(l,".").concat(d)]||m[d]||u[d]||a;return o?n.createElement(f,i(i({ref:t},c),{},{components:o})):n.createElement(f,i({ref:t},c))}));function d(e,t){var o=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var a=o.length,i=new Array(a);i[0]=m;var s={};for(var l in t)hasOwnProperty.call(t,l)&&(s[l]=t[l]);s.originalType=e,s.mdxType="string"==typeof e?e:r,i[1]=s;for(var p=2;p<a;p++)i[p]=o[p];return n.createElement.apply(null,i)}return n.createElement.apply(null,o)}m.displayName="MDXCreateElement"},5007:(e,t,o)=>{o.r(t),o.d(t,{assets:()=>l,contentTitle:()=>i,default:()=>u,frontMatter:()=>a,metadata:()=>s,toc:()=>p});var n=o(7462),r=(o(7294),o(3905));const a={sidebar_position:1},i="Introduction",s={unversionedId:"getting-started/introduction",id:"getting-started/introduction",title:"Introduction",description:"Cocos2D-Mono",source:"@site/docs/getting-started/introduction.md",sourceDirName:"getting-started",slug:"/getting-started/introduction",permalink:"/docs/getting-started/introduction",draft:!1,tags:[],version:"current",sidebarPosition:1,frontMatter:{sidebar_position:1},sidebar:"tutorialSidebar",previous:{title:"Getting Started",permalink:"/docs/category/getting-started"},next:{title:"Environment Setup",permalink:"/docs/getting-started/environment-setup"}},l={},p=[{value:"Tests",id:"tests",level:2},{value:"Linux &amp; macOS (OpenGL)",id:"linux--macos-opengl",level:2}],c={toc:p};function u(e){let{components:t,...o}=e;return(0,r.kt)("wrapper",(0,n.Z)({},c,o,{components:t,mdxType:"MDXLayout"}),(0,r.kt)("h1",{id:"introduction"},"Introduction"),(0,r.kt)("p",null,(0,r.kt)("img",{parentName:"p",src:"https://raw.githubusercontent.com/brandmooffin/cocos2d-mono/master/Logos/logo-full-200.png",alt:"Cocos2D-Mono"})),(0,r.kt)("p",null,"Cocos2D-Mono is the premier 2D game development engine based upon the wildly popular and successful Cocos2D-X engine and picking up where Cocos2D-XNA left off. With Cocos2D-Mono, the game developer can create fantastic games with rich user experiences without the tremendous cost of a proprietary game library. MIT licensed, and open source hosted on GitHub, this framework gives developers total power and control over every aspect of their game. Cocos2D-XNA has been used to deploy games to nearly every type of device in use today using XNA from Microsoft or MonoGame, Cocos2D-Mono hopes to continue that journey. Refreshing the Cocos2D-XNA project to support latest MonoGame versions and bringing support to more platforms, the power of XNA and the depth of Cocos2d are at every game developers reach -again-, taking their creative genius to over 95% of the computing devices on the planet."),(0,r.kt)("p",null,"Cocos2D-Mono focuses more on the MonoGame Framework and removes the limitations from proper XNA which held the original Cocos2D-XNA project back."),(0,r.kt)("h1",{id:"build-status"},"Build Status"),(0,r.kt)("p",null,(0,r.kt)("a",{parentName:"p",href:"https://github.com/brandmooffin/cocos2d-mono/actions/workflows/desktopgl_build.yml"},(0,r.kt)("img",{parentName:"a",src:"https://github.com/brandmooffin/cocos2d-mono/actions/workflows/desktopgl_build.yml/badge.svg",alt:"DesktopGL (Windows/Linux/macOS)"})),"\n",(0,r.kt)("a",{parentName:"p",href:"https://github.com/brandmooffin/cocos2d-mono/actions/workflows/windows_build.yml"},(0,r.kt)("img",{parentName:"a",src:"https://github.com/brandmooffin/cocos2d-mono/actions/workflows/windows_build.yml/badge.svg",alt:"Windows"})),"\n",(0,r.kt)("a",{parentName:"p",href:"https://github.com/brandmooffin/cocos2d-mono/actions/workflows/android_build.yml"},(0,r.kt)("img",{parentName:"a",src:"https://github.com/brandmooffin/cocos2d-mono/actions/workflows/android_build.yml/badge.svg",alt:"Android"})),"\n",(0,r.kt)("a",{parentName:"p",href:"https://github.com/brandmooffin/cocos2d-mono/actions/workflows/uwp_build.yml"},(0,r.kt)("img",{parentName:"a",src:"https://github.com/brandmooffin/cocos2d-mono/actions/workflows/uwp_build.yml/badge.svg",alt:"UWP"})),"\n",(0,r.kt)("a",{parentName:"p",href:"https://github.com/brandmooffin/cocos2d-mono/actions/workflows/ios_build.yml"},(0,r.kt)("img",{parentName:"a",src:"https://github.com/brandmooffin/cocos2d-mono/actions/workflows/ios_build.yml/badge.svg",alt:"iOS"}))),(0,r.kt)("h1",{id:"supported-platforms"},"Supported Platforms"),(0,r.kt)("p",null,"We support a growing list of platforms across the desktop, mobile, and console space. If there is a platform we don't support, please ",(0,r.kt)("a",{parentName:"p",href:"https://github.com/brandmooffin/cocos2d-mono/issues"},"make a request"),"."),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},"Desktop PCs",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"Windows 10 Store Apps (UWP)"),(0,r.kt)("li",{parentName:"ul"},"Windows Win32 (OpenGL & DirectX)"),(0,r.kt)("li",{parentName:"ul"},"Linux (OpenGL)"),(0,r.kt)("li",{parentName:"ul"},"macOS (OpenGL)"))),(0,r.kt)("li",{parentName:"ul"},"Mobile/Tablet Devices",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"Android (OpenGL)"),(0,r.kt)("li",{parentName:"ul"},"iOS (OpenGL)"),(0,r.kt)("li",{parentName:"ul"},"Windows Phone 10 (UWP)"))),(0,r.kt)("li",{parentName:"ul"},"Consoles",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"Xbox One (UWP)"))),(0,r.kt)("li",{parentName:"ul"},"Coming Soon",(0,r.kt)("ul",{parentName:"li"},(0,r.kt)("li",{parentName:"ul"},"iOS (Metal)"),(0,r.kt)("li",{parentName:"ul"},"tvOS (Metal)"),(0,r.kt)("li",{parentName:"ul"},"macOS (Metal)"),(0,r.kt)("li",{parentName:"ul"},"Xbox One (XDK)"),(0,r.kt)("li",{parentName:"ul"},"Nintendo Switch"),(0,r.kt)("li",{parentName:"ul"},"PlayStation Vita"),(0,r.kt)("li",{parentName:"ul"},"PlayStation 4")))),(0,r.kt)("h1",{id:"support--contributing"},"Support & Contributing"),(0,r.kt)("p",null,"If you think you have found a bug or have a feature request, use the ",(0,r.kt)("a",{parentName:"p",href:"https://github.com/brandmooffin/Cocos2D-Mono/issues"},"issue tracker"),". Before opening a new issue, please search to see if your problem has already been reported. Try to be as detailed as possible in your issue reports."),(0,r.kt)("p",null,"If you are interested in contributing fixes or features to Cocos2D-Mono, please read our ",(0,r.kt)("a",{parentName:"p",href:"/docs/contributing/getting-involved"},"contributors guide")," first."),(0,r.kt)("h2",{id:"tests"},"Tests"),(0,r.kt)("p",null,"We have created solutions for all the supported platforms that serves as our Test Bed for each platform."),(0,r.kt)("p",null,"You can find those in the ",(0,r.kt)("a",{parentName:"p",href:"https://github.com/brandmooffin/cocos2d-mono/tree/master/Tests",title:"Tests"},"Tests directory")),(0,r.kt)("ul",null,(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("p",{parentName:"li"},"cocos2d-mono.Tests.Android")),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("p",{parentName:"li"},"cocos2d-mono.Tests.Windows")),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("p",{parentName:"li"},"cocos2d-mono.Tests.Uwp")),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("p",{parentName:"li"},"cocos2d-mono.Tests.DesktopGL")),(0,r.kt)("li",{parentName:"ul"},(0,r.kt)("p",{parentName:"li"},"cocos2d-mono.Tests.iOS"))),(0,r.kt)("p",null,(0,r.kt)("strong",{parentName:"p"},"LINUX SETUP NOTE:")," There are some fonts used within the Test Bed not natively found on Linux, please run the following command to add the missing fonts:"),(0,r.kt)("blockquote",null,(0,r.kt)("p",{parentName:"blockquote"},"sudo apt-get install ttf-mscorefonts-installer")),(0,r.kt)("p",null,"More tests coming soon!"),(0,r.kt)("h2",{id:"linux--macos-opengl"},"Linux & macOS (OpenGL)"),(0,r.kt)("p",null,"For Linux & macOS projects use DesktopGL (cross-platform with Windows support)."))}u.isMDXComponent=!0}}]);