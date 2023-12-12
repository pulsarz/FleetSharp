using FleetSharp;
using FleetSharp.Builder;
using FleetSharp.Types;
using Newtonsoft.Json.Linq;
using System.Net;
using Xunit;

using static FleetSharp.Sigma.ConstantSerializer;
using static FleetSharp.Sigma.ISigmaCollection;
using static FleetSharp.Sigma.IPrimitiveSigmaType;
using System;
using System.Text.Json;
using System.Xml.Linq;
using FleetSharp.Tests.TestVectors;
using FleetSharp.Exceptions;

public class OutputBuilderTests
{
    private const long SAFE_MIN_BOX_VALUE = OutputBuilder.SAFE_MIN_BOX_VALUE;
    private const string address = "9fMPy1XY3GW4T6t3LjYofqmzER6x9cV21n5UVJTWmma4Y9mAW6c";
    private const string ergoTreeHex = "0008cd026dc059d64a50d0dbf07755c2c4a4e557e3df8afa7141868b3ab200643d437ee7";
    private const int height = 816992;

    private const string tokenA = "1fd6e032e8476c4aa54c18c1a308dce83940e8f4a28f576440513ed7326ad489";
    private const string tokenB = "bf59773def7e08375a553be4cbd862de85f66e6dd3dccb8f87f53158f9255bf5";

    [Fact]
    public void Constructor_ShouldConstructUsingRecipientParamAddressAsBase58()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height);

        Assert.Equal(SAFE_MIN_BOX_VALUE, builder.GetValue());
        Assert.Equal(address, builder.GetAddress().encode(Network.Mainnet));
        Assert.Equal(ergoTreeHex, builder.GetErgoTree());
        Assert.Equal(height, builder.GetCreationHeight());
    }

    [Fact]
    public void Constructor_ShouldConstructWithNoCreationHeight()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address));

        Assert.Equal(SAFE_MIN_BOX_VALUE, builder.GetValue());
        Assert.Equal(address, builder.GetAddress().encode(Network.Mainnet));
        Assert.Equal(ergoTreeHex, builder.GetErgoTree());
        Assert.Null(builder.GetCreationHeight());
    }

    [Fact]
    public void Constructor_ShouldConstructUsingRecipientParamAsErgoTreeHexString()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromErgoTree(ergoTreeHex, Network.Mainnet), height);

        Assert.Equal(SAFE_MIN_BOX_VALUE, builder.GetValue());
        Assert.Equal(address, builder.GetAddress().encode(Network.Mainnet));
        Assert.Equal(ergoTreeHex, builder.GetErgoTree());
        Assert.Equal(height, builder.GetCreationHeight());
    }

    [Fact]
    public void Constructor_ShouldConstructWithNoCreationHeightAndSetItUsingSetCreationHeight()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address));

        Assert.Null(builder.GetCreationHeight());

        builder.SetCreationHeight(10);
        Assert.Equal(10, builder.GetCreationHeight());

        builder.SetCreationHeight(height);
        Assert.Equal(height, builder.GetCreationHeight());
    }

    [Fact]
    public void Constructor_ShouldConstructUsingRecipientParamAsErgoAddress()
    {
        var ergoAddress = ErgoAddress.fromBase58(address);
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ergoAddress, height);

        Assert.Equal(ergoAddress, builder.GetAddress());
    }

    [Fact]
    public void Constructor_ShouldFailIfValueIsLessThanOrEqualToZero()
    {
        Assert.ThrowsAny<Exception>(() =>
            new OutputBuilder(0, ErgoAddress.fromBase58(address), height)
        );

        Assert.ThrowsAny<Exception>(() =>
            new OutputBuilder(-1, ErgoAddress.fromBase58(address), height)
        );
    }

    [Fact]
    public void CreationHeight_ShouldReplaceCreationHeight()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height);
        Assert.Equal(height, builder.GetCreationHeight());

        builder.SetCreationHeight(10);
        Assert.Equal(10, builder.GetCreationHeight()); // should replace by default

        builder.SetCreationHeight(11, true);
        Assert.Equal(11, builder.GetCreationHeight()); // should replace explicitly
    }

    [Fact]
    public void CreationHeight_ShouldNotReplaceCreationHeight()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address));
        Assert.Null(builder.GetCreationHeight()); // should be the same as constructor (undefined on this case)

        builder.SetCreationHeight(height, false);
        Assert.Equal(height, builder.GetCreationHeight()); // should set height since creationsHeight is undefined

        builder.SetCreationHeight(11, false);
        Assert.Equal(height, builder.GetCreationHeight()); // should not replace
    }

    [Fact]
    public void TokenHandling_ShouldAddDistinctTokens()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height);

        builder.AddTokens(new List<TokenAmount<long>> {
            new TokenAmount<long> { tokenId = tokenA, amount = 50 },
            new TokenAmount<long> { tokenId = tokenB, amount = 10 }
        });
        Assert.Equal(2, builder.GetAssets().Count);

        var tokens = builder.GetAssets();
        Assert.Equal(50, tokens.FirstOrDefault(x => x.tokenId == tokenA)?.amount);
        Assert.Equal(10, tokens.FirstOrDefault(x => x.tokenId == tokenB)?.amount);
    }

    [Fact]
    public void TokenHandling_ShouldSumIfTheSameTokenIdIsAddedMoreThanOneTime()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height);
        
        builder.AddTokens(new List<TokenAmount<long>> {
            new TokenAmount<long> { tokenId = tokenA, amount = 50 },
            new TokenAmount<long> { tokenId = tokenB, amount = 10 }
        });
        Assert.Equal(2, builder.GetAssets().Count);
        Assert.Equal(50, builder.GetAssets().FirstOrDefault(x => x.tokenId == tokenA)?.amount);

        builder.AddTokens(new List<TokenAmount<long>> {
            new TokenAmount<long> { tokenId = tokenA, amount = 100 }
        });
        Assert.Equal(2, builder.GetAssets().Count);
        Assert.Equal(150, builder.GetAssets().FirstOrDefault(x => x.tokenId == tokenA)?.amount);
        Assert.Equal(10, builder.GetAssets().FirstOrDefault(x => x.tokenId == tokenB)?.amount);
    }

    [Fact]
    public void TokenHandling_ShouldAddMultipleTokensAndSumIfTheSameTokenIdIsAddedMoreThenOneTime()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height);

        builder.AddTokens(new List<TokenAmount<long>> {
            new TokenAmount<long> { tokenId = tokenA, amount = 50 }
        });
        Assert.Single(builder.GetAssets());
        Assert.Equal(50, builder.GetAssets().FirstOrDefault(x => x.tokenId == tokenA)?.amount);

        builder.AddTokens(new List<TokenAmount<long>> {
            new TokenAmount<long> { tokenId = tokenA, amount = 100 },
            new TokenAmount<long> { tokenId = tokenB, amount = 10 }
        });
        Assert.Equal(2, builder.GetAssets().Count);
        Assert.Equal(150, builder.GetAssets().FirstOrDefault(x => x.tokenId == tokenA)?.amount);
        Assert.Equal(10, builder.GetAssets().FirstOrDefault(x => x.tokenId == tokenB)?.amount);
    }

    [Fact]
    public void TokenHandling_ShouldNotSumIfTheSameTokenIdIsAddedMoreThenOneTime()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height);

        builder.AddTokens(new List<TokenAmount<long>> {
            new TokenAmount<long> { tokenId = tokenA, amount = 50 }
        });
        Assert.Single(builder.GetAssets());
        Assert.Equal(50, builder.GetAssets().FirstOrDefault(x => x.tokenId == tokenA)?.amount);

        builder.AddTokens(new List<TokenAmount<long>> {
            new TokenAmount<long> { tokenId = tokenA, amount = 110 },
            new TokenAmount<long> { tokenId = tokenB, amount = 10 }
        }, sum: false);

        Assert.Equal(3, builder.GetAssets().Count);
        Assert.Contains(builder.GetAssets(), x => x.tokenId == tokenA && x.amount == 50);
        Assert.Contains(builder.GetAssets(), x => x.tokenId == tokenB && x.amount == 10);
        Assert.Contains(builder.GetAssets(), x => x.tokenId == tokenA && x.amount == 110);
    }

    [Fact]
    public void AdditionalRegisters_ShouldBindAdditionalRegistersProperly()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height);

        builder.SetAdditionalRegisters(new NonMandatoryRegisters
        {
            R4 = "0580c0fc82aa02",
            R5 = "0e240008cd036b84756b351ee1c57fd8c302e66a1bb927e5d8b6e1a8e085935de3971f84ae17",
            R6 = "07036b84756b351ee1c57fd8c302e66a1bb927e5d8b6e1a8e085935de3971f84ae17",
            R7 = SConstant(SBool(true)),
            R8 = null // should eliminate undefined properties
        });

        Assert.Equal(JsonSerializer.Serialize(new NonMandatoryRegisters
        {
            R4 = "0580c0fc82aa02",
            R5 = "0e240008cd036b84756b351ee1c57fd8c302e66a1bb927e5d8b6e1a8e085935de3971f84ae17",
            R6 = "07036b84756b351ee1c57fd8c302e66a1bb927e5d8b6e1a8e085935de3971f84ae17",
            R7 = "0101"
        }), JsonSerializer.Serialize(builder.GetAdditionalRegisters()));

        Assert.Null(Record.Exception(() => builder.SetAdditionalRegisters(new NonMandatoryRegisters
        {
            R4 = "0580c0fc82aa02",
            R5 = "0e240008cd036b84756b351ee1c57fd8c302e66a1bb927e5d8b6e1a8e085935de3971f84ae17",
            R6 = SConstant(SInt(0))
        })));

        Assert.Null(Record.Exception(() => builder.SetAdditionalRegisters(new NonMandatoryRegisters
        {
            R4 = "0580c0fc82aa02"
        })));

        Assert.Null(Record.Exception(() => builder.SetAdditionalRegisters(new NonMandatoryRegisters
        {
            R4 = SConstant(SInt(20))
        })));
    }


    /*
    * Registers should be densely packed. It's not possible to use
    * R9 without adding register R4 to R8, for example.
    */
    [Fact]
    public void AdditionalRegisters_ShouldThrowIfSomeRegisterIsSkipped()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height);

        Assert.Throws<InvalidRegistersPackingException>(() =>
            builder.SetAdditionalRegisters(new NonMandatoryRegisters
            {
                R5 = "0580c0fc82aa02"
            })
        );

        Assert.Throws<InvalidRegistersPackingException>(() =>
            builder.SetAdditionalRegisters(new NonMandatoryRegisters
            {
                R4 = "0580c0fc82aa02",
                R6 = "0580c0fc82aa02"
            })
        );

        Assert.Throws<InvalidRegistersPackingException>(() =>
            builder.SetAdditionalRegisters(new NonMandatoryRegisters
            {
                R4 = "0580c0fc82aa02",
                R5 = null,
                R6 = "0580c0fc82aa02"
            })
        );
    }

    [Fact]
    public void Building_ShouldBuildBoxWithoutTokens()
    {
        var boxCandidate = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height).build();

        Assert.Equal(SAFE_MIN_BOX_VALUE, boxCandidate.value);
        Assert.Equal(ErgoAddress.fromBase58(address).GetErgoTreeHex(), boxCandidate.ergoTree);
        Assert.Equal(height, boxCandidate.creationHeight);
        Assert.Empty(boxCandidate.assets);
        Assert.Equal(JsonSerializer.Serialize(new NonMandatoryRegisters()), JsonSerializer.Serialize(boxCandidate.additionalRegisters));
    }

    [Fact]
    public void Building_ShouldBuildBoxWithTokens()
    {
        var boxCandidate = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height)
            .AddTokens(new List<TokenAmount<long>> { new TokenAmount<long> { tokenId = tokenA, amount = 15 } })
            .AddTokens(new List<TokenAmount<long>> { new TokenAmount<long> { tokenId = tokenB, amount = 1 } })
            .build();

        Assert.Equal(SAFE_MIN_BOX_VALUE, boxCandidate.value);
        Assert.Equal(ErgoAddress.fromBase58(address).GetErgoTreeHex(), boxCandidate.ergoTree);
        Assert.Equal(height, boxCandidate.creationHeight);
        Assert.Equal(JsonSerializer.Serialize(new List<TokenAmount<long>> {
            new TokenAmount<long> { tokenId = tokenA, amount = 15 },
            new TokenAmount<long> { tokenId = tokenB, amount = 1 }
        }), JsonSerializer.Serialize(boxCandidate.assets));
        Assert.Equal(JsonSerializer.Serialize(new NonMandatoryRegisters()), JsonSerializer.Serialize(boxCandidate.additionalRegisters));
    }

    [Fact]
    public void Building_ShouldBuildBoxWithMintingToken()
    {
        var boxCandidate = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height)
            .mintToken(new NewToken<long> {
                amount = 100,
                name = "TestToken",
                decimals = 4,
                description = "Description test"
            })
            .build(MockedBoxes.RegularInputBoxes);

        Assert.Equal(SAFE_MIN_BOX_VALUE, boxCandidate.value);
        Assert.Equal(ErgoAddress.fromBase58(address).GetErgoTreeHex(), boxCandidate.ergoTree);
        Assert.Equal(height, boxCandidate.creationHeight);
        Assert.Equal(JsonSerializer.Serialize(new List<TokenAmount<long>> {
            new TokenAmount<long>
            {
                tokenId = MockedBoxes.RegularInputBoxes[0].boxId, // should be the same as the first input
                amount = 100
            }
        }), JsonSerializer.Serialize(boxCandidate.assets));
        Assert.Equal(JsonSerializer.Serialize(new NonMandatoryRegisters
        {
            R4 = "0e0954657374546f6b656e",
            R5 = "0e104465736372697074696f6e2074657374",
            R6 = "0e0134"
        }), JsonSerializer.Serialize(boxCandidate.additionalRegisters));
    }

    [Fact]
    public void Building_ShouldBuildBoxWithTokensAndMinting()
    {
        var boxCandidate = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height)
            .AddTokens(new List<TokenAmount<long>>
            {
                new TokenAmount<long> { tokenId = tokenA, amount = 15 },
                new TokenAmount<long> { tokenId = tokenB, amount = 1 }
            })
            .mintToken(new NewToken<long>
            {
                amount = 100,
                name = "TestToken"
            })
            .build(MockedBoxes.RegularInputBoxes);

        Assert.Equal(SAFE_MIN_BOX_VALUE, boxCandidate.value);
        Assert.Equal(ErgoAddress.fromBase58(address).GetErgoTreeHex(), boxCandidate.ergoTree);
        Assert.Equal(height, boxCandidate.creationHeight);
        Assert.Equal(JsonSerializer.Serialize(new List<TokenAmount<long>> {
            new TokenAmount<long>
            {
                tokenId = MockedBoxes.RegularInputBoxes[0].boxId, // should be the same as the first input
                amount = 100
            },
            new TokenAmount<long> { tokenId = tokenA, amount = 15 },
            new TokenAmount<long> { tokenId = tokenB, amount = 1 }
        }), JsonSerializer.Serialize(boxCandidate.assets));
        Assert.Equal(JsonSerializer.Serialize(new NonMandatoryRegisters
        {
            R4 = "0e0954657374546f6b656e",
            R5 = "0e00", // should be empty string
            R6 = "0e0130" // should be zero
        }), JsonSerializer.Serialize(boxCandidate.additionalRegisters));
    }

    [Fact]
    public void Building_ShouldAddDefaultValuesIfNonMandatoryMintingFieldsAreMissing()
    {
        var boxCandidate = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height)
            .mintToken(new NewToken<long>
            {
                amount = 100
            })
            .build(MockedBoxes.RegularInputBoxes);

        Assert.Equal(SAFE_MIN_BOX_VALUE, boxCandidate.value);
        Assert.Equal(ErgoAddress.fromBase58(address).GetErgoTreeHex(), boxCandidate.ergoTree);
        Assert.Equal(height, boxCandidate.creationHeight);
        Assert.Equal(JsonSerializer.Serialize(new List<TokenAmount<long>> {
            new TokenAmount<long>
            {
                tokenId = MockedBoxes.RegularInputBoxes[0].boxId, // should be the same as the first input
                amount = 100
            }
        }), JsonSerializer.Serialize(boxCandidate.assets));
        Assert.Equal(JsonSerializer.Serialize(new NonMandatoryRegisters
        {
            R4 = "0e00", // should be empty string
            R5 = "0e00", // should be empty string
            R6 = "0e0130" // should be zero
        }), JsonSerializer.Serialize(boxCandidate.additionalRegisters));
    }

    [Fact]
    public void Building_ShouldPreserveExplicitlyDefinedRegisters()
    {
        var boxCandidate = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height)
            .mintToken(new NewToken<long>
            {
                amount = 100,
                name = "TestToken"
            })
            .SetAdditionalRegisters(new NonMandatoryRegisters { R4 = "0e00" })
            .build(MockedBoxes.RegularInputBoxes);

        Assert.Equal(SAFE_MIN_BOX_VALUE, boxCandidate.value);
        Assert.Equal(ErgoAddress.fromBase58(address).GetErgoTreeHex(), boxCandidate.ergoTree);
        Assert.Equal(height, boxCandidate.creationHeight);
        Assert.Equal(JsonSerializer.Serialize(new List<TokenAmount<long>> {
            new TokenAmount<long>
            {
                tokenId = MockedBoxes.RegularInputBoxes[0].boxId, // should be the same as the first input
                amount = 100
            }
        }), JsonSerializer.Serialize(boxCandidate.assets));
        Assert.Equal("0e00", boxCandidate.additionalRegisters.R4);
    }

    [Fact]
    public void Building_ShouldFailIfInputsArentIncluded()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address), height)
            .mintToken(new NewToken<long>
            {
                amount = 100,
                name = "TestToken",
                decimals = 4,
                description = "Description test"
            });

        Assert.Throws<UndefinedMintingContextException>(() =>
            builder.build()
        );
    }

    [Fact]
    public void Building_ShouldFailIfNoCreationHeightIsAdded()
    {
        var builder = new OutputBuilder(SAFE_MIN_BOX_VALUE, ErgoAddress.fromBase58(address));

        Assert.Throws<UndefinedCreationHeightException>(() =>
            builder.build()
        );
    }
}
