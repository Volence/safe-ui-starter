namespace CustomComponents

open Fable.React
open Fable.React.Props
open Fable.MaterialUI.Core
open Fable.MaterialUI.Props

module AppBar =
    let appBarStyle = [
        CSSProp.ZIndex "1201"
    ]

    let main title =
        appBar [
            Style appBarStyle
        ] [
            toolbar [] [
                typography [
                    TypographyProp.Variant TypographyVariant.H4
                    MaterialProp.Color ComponentColor.Inherit
                ] [
                    str title
                ]
            ]
        ]