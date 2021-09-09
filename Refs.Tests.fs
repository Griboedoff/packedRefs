module Refs.Tests

open System.IO
open System.Reflection
open Xunit

open Refs

[<Fact>]
let ``Parse empty file should return empty seq`` () : unit =
    Assert.Empty(parsePackedRefsLines [| "" |])

[<Fact>]
let ``Strings starts with # are ignored`` () : unit =
    Assert.Empty(parsePackedRefsLines [| "#it's a comment" |])

[<Fact>]
let ``Parse correct formatted line`` () : unit =
    let hash =
        "cc07136d669554cf46ca4e9ef1eab7361336e1c8"

    let id = "refs/remotes/origin/master"
    Assert.Equal({ Name = id; CommitObjectId = hash }, Seq.head (parsePackedRefsLines [| $"{hash} {id}" |]))

[<Fact>]
let ``No packed-refs file should return empty seq`` () : unit =
    Assert.Empty(readPackedRefs ".")

[<Fact>]
let ``Parse packed-refs from git folder`` () : unit =
    let packedRefs = readPackedRefs "SampleWithPackedRefs"

    Assert.Equal(
        [ { Name = "refs/heads/branch1"; CommitObjectId = "96242453340b084928a03e847e2ce19394964555" }
          { Name = "refs/heads/branch_2"; CommitObjectId = "eb7d69b8d5316b3222bdcd1720f1dc2032df55b4" }
          { Name = "refs/heads/master"; CommitObjectId = "eb7d69b8d5316b3222bdcd1720f1dc2032df55b4" } ],
        packedRefs
    )

[<Fact>]
let ``Parse refs from git folder`` () : unit =
    let refs = readRefs "SampleWithPackedRefs"
    Assert.Equal(4, Seq.length refs)
    Assert.Contains({ Name = "refs/heads/branchAfterPackRefs"; CommitObjectId = "31f54c84d9e69bc30613aafd7bb521830250fc36" }, refs)


