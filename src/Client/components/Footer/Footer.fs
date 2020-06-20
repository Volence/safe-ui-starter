namespace CustomComponents

open Fable.React.Props
open Fable.MaterialUI.Core
open Fable.MaterialUI.Props

module Footer =
    let sideBarWidth = "240px"

    let footerStyle = [
        CSSProp.Top "auto"
        CSSProp.Bottom "0"
        CSSProp.Width ("calc(100% - " + sideBarWidth + ")")
        CSSProp.BorderLeftColor "transparent"
    ]

    let main footerContents =
        appBar [
            Style footerStyle
            MaterialProp.Color ComponentColor.Inherit
        ] [
            toolbar [] [
                footerContents
            ]
        ]