"use strict";(self.webpackChunkdocs=self.webpackChunkdocs||[]).push([[508],{3905:(e,t,o)=>{o.d(t,{Zo:()=>c,kt:()=>m});var n=o(7294);function r(e,t,o){return t in e?Object.defineProperty(e,t,{value:o,enumerable:!0,configurable:!0,writable:!0}):e[t]=o,e}function a(e,t){var o=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),o.push.apply(o,n)}return o}function s(e){for(var t=1;t<arguments.length;t++){var o=null!=arguments[t]?arguments[t]:{};t%2?a(Object(o),!0).forEach((function(t){r(e,t,o[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(o)):a(Object(o)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(o,t))}))}return e}function i(e,t){if(null==e)return{};var o,n,r=function(e,t){if(null==e)return{};var o,n,r={},a=Object.keys(e);for(n=0;n<a.length;n++)o=a[n],t.indexOf(o)>=0||(r[o]=e[o]);return r}(e,t);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);for(n=0;n<a.length;n++)o=a[n],t.indexOf(o)>=0||Object.prototype.propertyIsEnumerable.call(e,o)&&(r[o]=e[o])}return r}var p=n.createContext({}),l=function(e){var t=n.useContext(p),o=t;return e&&(o="function"==typeof e?e(t):s(s({},t),e)),o},c=function(e){var t=l(e.components);return n.createElement(p.Provider,{value:t},e.children)},u={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},d=n.forwardRef((function(e,t){var o=e.components,r=e.mdxType,a=e.originalType,p=e.parentName,c=i(e,["components","mdxType","originalType","parentName"]),d=l(o),m=r,f=d["".concat(p,".").concat(m)]||d[m]||u[m]||a;return o?n.createElement(f,s(s({ref:t},c),{},{components:o})):n.createElement(f,s({ref:t},c))}));function m(e,t){var o=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var a=o.length,s=new Array(a);s[0]=d;var i={};for(var p in t)hasOwnProperty.call(t,p)&&(i[p]=t[p]);i.originalType=e,i.mdxType="string"==typeof e?e:r,s[1]=i;for(var l=2;l<a;l++)s[l]=o[l];return n.createElement.apply(null,s)}return n.createElement.apply(null,o)}d.displayName="MDXCreateElement"},5879:(e,t,o)=>{o.r(t),o.d(t,{assets:()=>p,contentTitle:()=>s,default:()=>u,frontMatter:()=>a,metadata:()=>i,toc:()=>l});var n=o(7462),r=(o(7294),o(3905));const a={slug:"2.4.3-release",title:"Cocos2D-Mono 2.4.3 is now out!",authors:["brandmooffin"],tags:["update","release","2.4.3"]},s=void 0,i={permalink:"/blog/2.4.3-release",source:"@site/blog/2023-08-05-release-2-4-3/index.md",title:"Cocos2D-Mono 2.4.3 is now out!",description:"Cocos2D-Mono 2.4.3 is out now! Go check out the release notes to see what's change...or just read below.",date:"2023-08-05T00:00:00.000Z",formattedDate:"August 5, 2023",tags:[{label:"update",permalink:"/blog/tags/update"},{label:"release",permalink:"/blog/tags/release"},{label:"2.4.3",permalink:"/blog/tags/2-4-3"}],readingTime:1.16,hasTruncateMarker:!1,authors:[{name:"Brandon",title:"Makes things",url:"https://github.com/brandmooffin",imageURL:"https://avatars.githubusercontent.com/u/1774581?v=4",key:"brandmooffin"}],frontMatter:{slug:"2.4.3-release",title:"Cocos2D-Mono 2.4.3 is now out!",authors:["brandmooffin"],tags:["update","release","2.4.3"]},prevItem:{title:"Cocos2D-Mono 2.4.5 is now out!",permalink:"/blog/2.4.5-release"},nextItem:{title:"Cocos2D-Mono 2.4.2 is now out!",permalink:"/blog/2.4.2-release"}},p={authorsImageUrls:[void 0]},l=[{value:"What&#39;s Changed",id:"whats-changed",level:2}],c={toc:l};function u(e){let{components:t,...o}=e;return(0,r.kt)("wrapper",(0,n.Z)({},c,o,{components:t,mdxType:"MDXLayout"}),(0,r.kt)("p",null,"Cocos2D-Mono 2.4.3 is out now! Go check out the ",(0,r.kt)("a",{parentName:"p",href:"https://github.com/brandmooffin/cocos2d-mono/releases/tag/2.4.3"},"release notes")," to see what's change...or just read below."),(0,r.kt)("h2",{id:"whats-changed"},"What's Changed"),(0,r.kt)("pre",null,(0,r.kt)("code",{parentName:"pre"},"- Support for AddedToScene for CCLayer\n- New CCTapNode for supporting TouchOneByOne TouchMode\n- Support Dispose function for CCNode, CCSprite, CCLayer\n- New CCActionState\n- Support Math Operators for CCColor4B\n- Support for initializing CCScale9Sprite from existing CCSprite\n- Added ContentSize getter for CCSpriteFrame\n    - Note: OriginalSize will be deprecated in a future release\n- Added TextureRectInPixels and SpriteFrame Properties for CCSprite\n- Added IsSpriteFrameDisplayed function for CCSprite\n    - Note: IsFrameDisplayed will be deprecated in a future release\n- New IsNear function for CCPoint\n- Added LengthSquared and DistanceSquared functions for CCPoint\n    - Note: DistanceSQ, LengthSQ and LengthSquare will be deprecated in a future release\n- New CCActions: CCColorBlendAnimation, CCMoveFrom, CCRotateAnimation, CCSwapAction, CCTargetedAction, CCTimerAction\n- Support for UnscheduleAll for CCNode\n- Added SystemFont and SystemFontSize property for CCLabel\n    - Note: FontName and FontSize will be deprecated in a future release\n- Added ScaledContentSize and BoundingBoxTransformedToWorld properties for CCNode\n- Migrated CCNodeRGBA into CCNode\n    - Note: CCNodeRGBA will be deprecated in a future release\n- Added Lerp and Clamp functions to CCMathHelper\n- New Pi properties for CCMathHelper\n")),(0,r.kt)("p",null,(0,r.kt)("strong",{parentName:"p"},"Full Changelog"),": ",(0,r.kt)("a",{parentName:"p",href:"https://github.com/brandmooffin/cocos2d-mono/compare/2.4.2...2.4.3"},"https://github.com/brandmooffin/cocos2d-mono/compare/2.4.2...2.4.3")),(0,r.kt)("p",null,"NuGet Packages:"),(0,r.kt)("p",null,(0,r.kt)("a",{parentName:"p",href:"https://www.nuget.org/packages/Cocos2D-Mono.Android/"},"Cocos2D-Mono.Android"),"\n",(0,r.kt)("a",{parentName:"p",href:"https://www.nuget.org/packages/Cocos2D-Mono.DesktopGL/"},"Cocos2D-Mono.DesktopGL"),"\n",(0,r.kt)("a",{parentName:"p",href:"https://www.nuget.org/packages/Cocos2D-Mono.iOS/"},"Cocos2D-Mono.iOS"),"\n",(0,r.kt)("a",{parentName:"p",href:"https://www.nuget.org/packages/Cocos2D-Mono.Uwp/"},"Cocos2D-Mono.Uwp"),"\n",(0,r.kt)("a",{parentName:"p",href:"https://www.nuget.org/packages/Cocos2D-Mono.Windows/"},"Cocos2D-Mono.Windows")),(0,r.kt)("p",null,(0,r.kt)("a",{parentName:"p",href:"https://marketplace.visualstudio.com/items?itemName=Cocos2D-MonoTeamBrokenWallsStudios.cocos2dmonoprojecttemplates"},"Visual Studio Project Template Extension")),(0,r.kt)("p",null,"This release sees some great improvements to the library! "),(0,r.kt)("p",null,"Check it out & stay tuned for more to come!"))}u.isMDXComponent=!0}}]);