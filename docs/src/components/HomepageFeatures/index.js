import React from "react";
import clsx from "clsx";
import styles from "./styles.module.css";

const FeatureList = [
  {
    title: "Focus on What Matters",
    Svg: require("@site/static/img/cocos2d-mono-small-logo.svg").default,
    description: (
      <>
        Cocos2d-Mono lets you focus on your games, and we&apos;ll do the chores.
        No need to worry about complicated game loops.
      </>
    ),
  },
  {
    title: "Easy to Use",
    Svg: require("@site/static/img/c-logo.svg").default,
    description: (
      <>
        Cocos2d-Mono was designed to build and develop games with C# and get up
        and running quickly.
      </>
    ),
  },
  {
    title: "Powered by MonoGame",
    Svg: require("@site/static/img/monogame-logo.svg").default,
    description: (
      <>
        Harness the power of MonoGame under the hood and build amazing games
        with a simpler approach.
      </>
    ),
  },
];

function Feature({ Svg, title, description }) {
  return (
    <div className={clsx("col col--4")}>
      <div className="text--center">
        <Svg className={styles.featureSvg} role="img" />
      </div>
      <div className="text--center padding-horiz--md">
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures() {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
